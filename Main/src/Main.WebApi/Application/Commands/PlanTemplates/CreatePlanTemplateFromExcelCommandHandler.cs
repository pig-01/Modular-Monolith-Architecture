using Base.Domain.Exceptions;
using Base.Infrastructure.Interface.Authentication;
using Base.Infrastructure.Interface.TimeZone;
using Base.Infrastructure.Toolkits.Extensions;
using Main.Domain.AggregatesModel.PlanTemplateAggregate;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.Dto.ViewModel.Common;
using Main.WebApi.Application.Commands.Bizform.Forms;
using Main.WebApi.Application.Models.Bizform.Forms;
using Main.WebApi.Application.Queries.PlanTemplates;
using Main.WebApi.Application.Queries.SystemCode;

namespace Main.WebApi.Application.Commands.PlanTemplates;

public class CreatePlanTemplateFromExcelCommandHandler(
    ILogger<CreatePlanTemplateFromExcelCommandHandler> logger,
    ITimeZoneService timeZoneService,
    IUserService<Scuser> userService,
    IPlanTemplateRepository planTemplateRepository,
    IPlanTemplateQuery planTemplateQuery,
    ISystemDataQuery systemQuery,
    IMediator mediator
) : IRequestHandler<CreatePlanTemplateFromExcelCommand, bool>
{
    public async Task<bool> Handle(CreatePlanTemplateFromExcelCommand request, CancellationToken cancellationToken)
    {
        DateTime now = timeZoneService.Now;
        Scuser currentUser = await userService.Now(cancellationToken);
        List<ViewSystemData> groups = (await systemQuery.GetSystemCode("DemoGroupId", cancellationToken)) ?? [];
        List<ViewSystemData> requestUnits = (await systemQuery.GetSystemCode("RequestUnit", cancellationToken)) ?? [];
        IEnumerable<GriRule> griRules = await planTemplateQuery.GetGriRulesAsync(cancellationToken);

        List<IGrouping<string?, Dto.ViewModel.PlanTemplate.ViewPlanTemplateExcelData>> planTemplateGroups = [.. request.DataList
           .GroupBy(x => x.PlanTemplateName)
           .Where(g => !string.IsNullOrEmpty(g.Key))];

        GetFormsCommand command = new()
        {
            Page = 1,
            PageSize = 9999
        };
        IEnumerable<Form> forms = await mediator.Send(command, cancellationToken) ?? throw new HandleException("Failed to retrieve forms");

        await planTemplateRepository.DeleteRangeByVersionAsync(request.Version, cancellationToken);

        int createdCount = 0;

        foreach (IGrouping<string?, Dto.ViewModel.PlanTemplate.ViewPlanTemplateExcelData>? group in planTemplateGroups)
        {
            Dto.ViewModel.PlanTemplate.ViewPlanTemplateExcelData firstItem = group.First();

            // Find group ID by name
            ViewSystemData? groupEntity = groups.FirstOrDefault(g => g.Name == firstItem.GroupName);
            if (groupEntity is null || groupEntity.SystemCode is null)
            {
                logger.LogWarning("Group not found: {GroupName}", firstItem.GroupName);
                continue;
            }

            // Find form ID by name
            Form? form = forms.FirstOrDefault(f => f.DisplayName == firstItem.FormName);
            long formId = 0;
            if (form is null || !form.Id.HasValue)
            {
                // TODO 先不移除匯入 EXCEL 中的 FormId 欄位，先當作 Default，之後再做對應
                // Use FormId from Excel data
                formId = firstItem.FormId ?? 0;
                logger.LogWarning("Form not found: {FormName}", firstItem.FormName);
            }
            else
            {
                formId = form.Id.Value;
            }

            // Create PlanTemplate
            PlanTemplate planTemplate = new()
            {
                PlanTemplateName = firstItem.PlanTemplateName!,
                PlanTemplateChName = firstItem.PlanTemplateChName!,
                PlanTemplateEnName = firstItem.PlanTemplateEnName!,
                PlanTemplateJpName = firstItem.PlanTemplateJpName!,
                FormId = (int)formId,
                FormName = firstItem.FormName,
                AcceptDataSource = firstItem.AcceptDataSource,
                Version = request.Version,
                GroupId = groupEntity.SystemCode,
                RowNumber = createdCount + 1,
                SortSequence = (createdCount + 1) * 10,
                CycleType = firstItem.CycleType,
                IsDeploy = firstItem.IsDeploy ?? false,
                CreatedDate = now,
                CreatedUser = currentUser.UserId,
                ModifiedDate = now,
                ModifiedUser = currentUser.UserId
            };

            logger.LogDebug("PlanTemplate: {PlanTemplate}", planTemplate.ToJson());
            PlanTemplate createdTemplate = await planTemplateRepository.AddAsync(planTemplate, cancellationToken);

            // Create planTemplateForm
            PlanTemplateForm planTemplateForm = new()
            {
                PlanTemplateId = createdTemplate.PlanTemplateId,
                FormId = formId,
                TenantId = currentUser.CurrentTenant.TenantId,
                CreatedDate = now,
                CreatedUser = currentUser.UserId,
                ModifiedDate = now,
                ModifiedUser = currentUser.UserId
            };

            await planTemplateRepository.AddFormAsync(planTemplateForm, cancellationToken);

            // Create planTemplateRequestUnit
            string[] requestUnitCodes = firstItem.RequestUnitIds?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? [];
            foreach (string unitCode in requestUnitCodes)
            {
                PlanTemplateRequestUnit planTemplateRequestUnit = new()
                {
                    PlanTemplateId = createdTemplate.PlanTemplateId,
                    UnitCode = unitCode.Trim()
                };

                logger.LogDebug("PlanTemplateRequestUnit: {PlanTemplateRequestUnit}", planTemplateRequestUnit.ToJson());
                await planTemplateRepository.AddRequestUnitAsync(planTemplateRequestUnit, cancellationToken);
            }

            // Create PlanTemplateDetails for this template
            foreach (Dto.ViewModel.PlanTemplate.ViewPlanTemplateExcelData? excelData in group)
            {
                if (string.IsNullOrEmpty(excelData.PlanTemplateDetailTitle))
                {
                    logger.LogWarning("PlanTemplateDetailTitle is empty for template: {TemplateName}", excelData.PlanTemplateName);
                    continue;
                }

                PlanTemplateDetail planTemplateDetail = new()
                {
                    PlanTemplateId = createdTemplate.PlanTemplateId,
                    Title = excelData.PlanTemplateDetailTitle,
                    ChTitle = excelData.PlanTemplateDetailChTitle,
                    EnTitle = excelData.PlanTemplateDetailEnTitle,
                    JpTitle = excelData.PlanTemplateDetailJpTitle,
                    SortSequence = (createdCount + 1) * 10,
                    CreatedDate = now,
                    CreatedUser = currentUser.UserId,
                    ModifiedDate = now,
                    ModifiedUser = currentUser.UserId
                };

                logger.LogDebug("PlanTemplateDetail: {PlanTemplateDetail}", planTemplateDetail.ToJson());
                PlanTemplateDetail createdDetail = await planTemplateRepository.AddPlanTemplateDetailAsync(planTemplateDetail, cancellationToken);

                // Create PlanTemplateDetailGriRules
                string[] griRuleCodes = excelData.GriRuleCodes?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? [];
                List<GriRule> filteredGriRules = [.. griRules.Where(rule => griRuleCodes.Contains(rule.Code))];

                foreach (GriRule? griRule in filteredGriRules)
                {
                    PlanTemplateDetailGriRule planTemplateDetailGriRule = new()
                    {
                        PlanTemplateDetailId = createdDetail.PlanTemplateDetailId,
                        GriRuleId = griRule.Id,
                        CreatedDate = now,
                        CreatedUser = currentUser.UserId,
                        ModifiedDate = now,
                        ModifiedUser = currentUser.UserId,
                    };

                    logger.LogDebug("PlanTemplateDetailGriRule: {PlanTemplateDetailGriRule}", planTemplateDetailGriRule.ToJson());
                    await planTemplateRepository.AddDetailGriRuleAsync(planTemplateDetailGriRule, cancellationToken);
                }

                // Create PlanTemplateDetailExposeIndustries
                string[] industryIds = excelData.ExposeIndustryIds?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? [];
                foreach (string industryId in industryIds)
                {
                    PlanTemplateDetailExposeIndustry planTemplateDetailExposeIndustry = new()
                    {
                        PlanTemplateDetailId = createdDetail.PlanTemplateDetailId,
                        IndustryId = industryId.Trim(),
                        CreatedDate = now,
                        CreatedUser = currentUser.UserId,
                        ModifiedDate = now,
                        ModifiedUser = currentUser.UserId
                    };

                    logger.LogDebug("PlanTemplateDetailExposeIndustry: {PlanTemplateDetailExposeIndustry}", planTemplateDetailExposeIndustry.ToJson());
                    await planTemplateRepository.AddDetailExposeIndustryAsync(planTemplateDetailExposeIndustry, cancellationToken);
                }
            }

            createdCount++;
        }

        return await Task.FromResult(true);
    }
}

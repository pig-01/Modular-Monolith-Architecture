using System.Globalization;
using Base.Domain.Exceptions;
using Base.Infrastructure.Interface.Authentication;
using Main.Domain.AggregatesModel.CustomTemplateAggregate;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.AggregatesModel.PlanTemplateAggregate;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.WebApi.Application.Queries.CustomTemplate;
using Main.WebApi.Application.Queries.Plans;
using Main.WebApi.Application.Queries.PlanTemplates;

namespace Main.WebApi.Application.Commands.Plans;

public class CreatePlanDetailCommandHandler(
    ILogger<CreatePlanDetailCommandHandler> logger,
    IPlanQuery planQuery,
    IPlanTemplateQuery planTemplateQuery,
    ICustomPlanTemplateQuery customPlanTemplateQuery,
    IPlanRepository planRepository,
    IUserService<Scuser> userService) : IRequestHandler<CreatePlanDetailCommand, bool>
{
    private const string DefaultCycleType = "year";
    private const bool DefaultCycleMonthLast = true;

    [Authorize(Policy = "User")]
    public async Task<bool> Handle(CreatePlanDetailCommand request, CancellationToken cancellationToken)
    {
        Scuser scuser = await userService.Now(cancellationToken);

        // Validate the request
        int planId = request.PlanId ?? throw new HandleException("PlanId is not found");

        // Check if the plan exists
        Plan plan = await planQuery.GetByIdAsync(planId, cancellationToken) ?? throw new HandleException("Plan not found");

        // Create a new PlanDetail from the specified template
        PlanDetail planDetail = (request.PlanTemplateId.HasValue, request.CustomPlanTemplateId.HasValue) switch
        {
            (true, true) => throw new HandleException("Only one of PlanTemplateId or CustomPlanTemplateId should be provided"),
            (false, true) => await CreatePlanDetailFromCustomTemplate(plan, request.CustomPlanTemplateId!.Value, request.RowNumber, request.Year!.Value, scuser, cancellationToken),
            (true, false) => await CreatePlanDetailFromTemplate(plan, request.PlanTemplateId.Value, request.RowNumber, request.Year!.Value, scuser, cancellationToken),
            (false, false) => throw new HandleException("Either PlanTemplateId or CustomPlanTemplateId must be provided")
        };

        // Add the new PlanDetail to the plan
        _ = await planRepository.AddPlanDetailAsync(planDetail, cancellationToken);

        return true;
    }

    /// <summary>
    /// Creates a new PlanDetail from the specified template.
    /// </summary>
    /// <param name="plan">The plan to which the detail belongs.</param>
    /// <param name="planTemplateId">The ID of the plan template.</param>
    /// <param name="rowNumber">The row number for the plan detail.</param>
    /// <param name="year">The year for the plan detail.</param>
    /// <param name="scuser">The user creating the plan detail.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    private async Task<PlanDetail> CreatePlanDetailFromTemplate(Plan plan, int planTemplateId, int rowNumber, int year, Scuser scuser, CancellationToken cancellationToken)
    {
        // Check if the plan template exists
        PlanTemplate planTemplate = await planTemplateQuery.GetByIdAsync(planTemplateId, cancellationToken) ?? throw new HandleException("PlanTemplate not found");

        // Get the form ID for the current tenant
        long formId = planTemplate.PlanTemplateForms.FirstOrDefault(x => x.TenantId == scuser.CurrentTenant.TenantId)?.FormId ?? throw new HandleException("Form not found");

        PlanDetail planDetail = new()
        {
            PlanId = plan.PlanId,
            PlanTemplateId = planTemplate.PlanTemplateId,
            PlanDetailName = planTemplate.PlanTemplateName,
            AcceptDataSource = planTemplate.AcceptDataSource,
            PlanDetailEnName = planTemplate.PlanTemplateEnName,
            PlanDetailChName = planTemplate.PlanTemplateChName,
            PlanDetailJpName = planTemplate.PlanTemplateJpName,
            RowNumber = rowNumber.ToString(CultureInfo.InvariantCulture),
            FormId = formId,
            EndDate = new DateTime(year, 12, 31),
            GroupId = planTemplate.GroupId,
            CycleMonth = 1,
            CycleType = planTemplate.CycleType ?? DefaultCycleType,
            CycleMonthLast = DefaultCycleMonthLast,
            Show = true,
            CreatedUser = scuser.UserId,
            ModifiedUser = scuser.UserId
        };

        planDetail.SetCreateMetadata(scuser.UserId, scuser.UserId);

        logger.LogDebug("PlanDetail: {@PlanDetail}", planDetail);

        return planDetail;
    }

    /// <summary>
    /// Creates a new PlanDetail from the specified custom template.
    /// </summary>
    /// <param name="plan">The plan to which the detail belongs.</param>
    /// <param name="customPlanTemplateId">The ID of the custom plan template.</param>
    /// <param name="rowNumber">The row number for the plan detail.</param>
    /// <param name="year">The year for the plan detail.</param>
    /// <param name="scuser">The user creating the plan detail.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    private async Task<PlanDetail> CreatePlanDetailFromCustomTemplate(Plan plan, int customPlanTemplateId, int rowNumber, int year, Scuser scuser, CancellationToken cancellationToken)
    {
        // Check if the custom plan template exists
        CustomPlanTemplate customPlanTemplate = await customPlanTemplateQuery.GetByIdAsync(customPlanTemplateId, cancellationToken) ?? throw new HandleException("CustomPlanTemplate not found");

        // Get the form ID for the current tenant
        long formId = customPlanTemplate.FormId ?? throw new HandleException("Form not found");

        PlanDetail planDetail = new()
        {
            PlanId = plan.PlanId,
            CustomPlanTemplateId = customPlanTemplate.PlanTemplateId,
            PlanDetailName = customPlanTemplate.PlanTemplateName,
            AcceptDataSource = "Bizform", // Custom templates always use bizform
            PlanDetailEnName = customPlanTemplate.PlanTemplateNameEn,
            PlanDetailChName = customPlanTemplate.PlanTemplateNameCh,
            PlanDetailJpName = customPlanTemplate.PlanTemplateNameJp,
            RowNumber = customPlanTemplate.Code ?? rowNumber.ToString(CultureInfo.InvariantCulture),
            FormId = formId,
            EndDate = new DateTime(year, 12, 31),
            GroupId = customPlanTemplate.GroupId,
            CycleType = customPlanTemplate.CycleType ?? DefaultCycleType,
            CycleMonth = 1,
            CycleMonthLast = DefaultCycleMonthLast,
            Show = true,
            CreatedUser = scuser.UserId,
            ModifiedUser = scuser.UserId
        };

        planDetail.SetCreateMetadata(scuser.UserId, scuser.UserId);

        logger.LogDebug("Custom PlanDetail: {@PlanDetail}", planDetail);

        return planDetail;
    }
}

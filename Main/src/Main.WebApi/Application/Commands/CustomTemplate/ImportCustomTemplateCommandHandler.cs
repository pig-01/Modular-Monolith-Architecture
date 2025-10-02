using Base.Domain.Exceptions;
using Base.Infrastructure.Interface.Authentication;
using Base.Infrastructure.Interface.Progress;
using Main.Domain.AggregatesModel.CustomTemplateAggregate;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.Dto.ViewModel.Common;
using Main.Dto.ViewModel.CustomTemplate;
using Main.WebApi.Application.Commands.Bizform.Forms;
using Main.WebApi.Application.Models.Bizform.Forms;
using Main.WebApi.Application.Queries.SystemCode;

namespace Main.WebApi.Application.Commands.CustomTemplate;

public class ImportCustomTemplateCommandHandler(
    ILogger<ImportCustomTemplateCommandHandler> logger,
    ISystemDataQuery systemDataQuery,
    IUserService<Scuser> userService,
    ICustomRequestUnitRepository customRequestUnitRepository,
    IBatchProgressService batchProgressService,
    IMediator mediator) : IRequestHandler<ImportCustomTemplateCommand, Unit?>
{
    /// <summary>
    /// 處理根據Excel資料重匯當前版本的CustomTemplates命令
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Unit?> Handle(ImportCustomTemplateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            GetFormsCommand command = new()
            {
                Page = 1,
                PageSize = 9999
            };
            IEnumerable<Form> forms = await mediator.Send(command, cancellationToken) ?? throw new HandleException("Failed to retrieve forms");

            List<ViewSystemData> industryList = await systemDataQuery.GetSystemCode("Industry", cancellationToken) ?? throw new HandleException("Failed to retrieve industry codes");

            List<ViewSystemData> DemoGroupList = await systemDataQuery.GetSystemCode("DemoGroupId", cancellationToken) ?? throw new HandleException("Failed to retrieve 專案group codes");

            Scuser currentUser = await userService.Now(cancellationToken);
            await batchProgressService.ReportProgress(request.TaskId, 0, "plan_template_custom.fileupload_import_starting");

            // Validate Excel Data
            if (request.ExcelData is null || !request.ExcelData.Any())
            {
                await batchProgressService.ReportError<IEnumerable<ErrorExcelData>>(request.TaskId, "plan_template_custom.fileupload_import_failed");
                return null;
            }

            List<ErrorExcelData> validationErrors = ValidateExcelData(request.ExcelData, forms);
            if (validationErrors.Count is not 0)
            {
                await batchProgressService.ReportError<IEnumerable<ErrorExcelData>>(request.TaskId, "plan_template_custom.fileupload_import_failed", validationErrors);
                return null;
            }

            await batchProgressService.ReportProgress(request.TaskId, 10, "plan_template_custom.fileupload_import_validation_passed");

            // Process Data by PlanTemplateName groups
            List<IGrouping<string?, ViewCustomTemplateExcelData>> groupedData = [.. request.ExcelData
                .Where(x => !string.IsNullOrWhiteSpace(x.PlanTemplateName))
                .GroupBy(x => x.PlanTemplateName)];

            int totalGroups = groupedData.Count;
            int processedGroups = 0;

            logger.LogInformation("Starting import of custom templates, Task ID: {TaskId}, Total Templates: {TotalGroups}",
                request.TaskId, totalGroups);

            int sortSequence = 1;
            foreach (IGrouping<string?, ViewCustomTemplateExcelData> group in groupedData)
            {
                string planTemplateName = group.Key!;
                List<ViewCustomTemplateExcelData> details = [.. group];

                // Get template info from first record in group
                ViewCustomTemplateExcelData firstRecord = details.First();
                string groupName = MapGroupNameToId(firstRecord.GroupName, DemoGroupList);
                string code = firstRecord.Code ?? "";
                string formName = firstRecord.FormName ?? "";
                string cycleType = firstRecord.CycleType ?? "";
                long formId = firstRecord.FormId;

                // Create template with version
                CustomPlanTemplate customTemplate = new(
                    planTemplateName,
                    firstRecord.PlanTemplateNameCh,
                    firstRecord.PlanTemplateNameEn,
                    firstRecord.PlanTemplateNameJp,
                    formId,
                    formName,
                    groupName,
                    cycleType,
                    code,
                    sortSequence++,
                    currentUser.UserId)
                {
                    VersionId = request.VersionId,
                };

                // Add details to template
                int detailSortSequence = 1;
                foreach (ViewCustomTemplateExcelData detail in details)
                {
                    if (!string.IsNullOrWhiteSpace(detail.PlanTemplateDetailTitle))
                    {
                        // Map industry names to IDs
                        string[] industryIds = MapIndustryNamesToIds(detail.ExposeIndustries, industryList);

                        customTemplate.AddDetail(
                            detail.PlanTemplateDetailTitle,
                            detail.PlanTemplateDetailTitleCh,
                            detail.PlanTemplateDetailTitleEn,
                            detail.PlanTemplateDetailTitleJp,
                            detailSortSequence++,
                            industryIds,
                            currentUser.UserId);
                    }
                }

                // Save template to database
                _ = await customRequestUnitRepository.AddCustomTemplateAsync(customTemplate, cancellationToken);

                processedGroups++;
                int progressPercentage = 10 + (int)((double)processedGroups / totalGroups * 80); // 10-90%
                await batchProgressService.ReportProgress(request.TaskId, progressPercentage,
                    $"plan_template_custom.fileupload_import_processing");

                logger.LogDebug("Processed template: {PlanTemplateName}, Detail count: {DetailCount}",
                    planTemplateName, customTemplate.CustomPlanTemplateDetails.Count);
            }

            await batchProgressService.ReportComplete(request.TaskId, $"plan_template_custom.fileupload_import_completed");

            logger.LogInformation("Import of custom templates completed, Task ID: {TaskId}, Processed Templates: {TemplateCount}",
                request.TaskId, totalGroups);

            return Unit.Value;
        }
        catch (Exception ex)
        {
            await batchProgressService.ReportError<IEnumerable<ErrorExcelData>>(request.TaskId, $"An error occurred during import: {ex.Message}");
            logger.LogError(ex, "Import of custom templates failed, Task ID: {TaskId}", request.TaskId);
            throw;
        }
    }

    /// <summary>
    /// Maps group name to its corresponding ID.
    /// </summary>
    /// <param name="groupName"></param>
    /// <param name="DemoGroupList"></param>
    /// <returns></returns>
    private static string MapGroupNameToId(string? groupName, IEnumerable<ViewSystemData> DemoGroupList)
    {
        if (string.IsNullOrWhiteSpace(groupName))
            return "";

        ViewSystemData? systemData = DemoGroupList.FirstOrDefault(i => i.Name == groupName);
        return systemData?.SystemCode ?? "";
    }

    /// <summary>
    /// Maps industry names to their corresponding IDs.
    /// </summary>
    /// <param name="exposeIndustries"></param>
    /// <param name="industryList"></param>
    /// <returns></returns>
    private static string[] MapIndustryNamesToIds(string? exposeIndustries, IEnumerable<ViewSystemData> industryList)
    {
        if (string.IsNullOrWhiteSpace(exposeIndustries))
            return [];

        List<string> industryIds = [];
        string[] industries = exposeIndustries!.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (string industry in industries)
        {
            if (industry is "All")
            {
                industryIds.Add(industry);
                break;
            }

            ViewSystemData? systemData = industryList.FirstOrDefault(i => i.Name == industry);
            if (systemData != null && !string.IsNullOrEmpty(systemData.SystemCode))
            {
                industryIds.Add(systemData.SystemCode!);
            }
        }
        return [.. industryIds];
    }

    /// <summary>
    /// Validates the Excel data for required fields and correct formats.
    /// </summary>
    /// <param name="excelData"></param>
    /// <param name="forms"></param>
    /// <returns></returns>
    private static List<ErrorExcelData> ValidateExcelData(IEnumerable<ViewCustomTemplateExcelData> excelData, IEnumerable<Form> forms)
    {
        List<ErrorExcelData> errorList = [];

        int rowIndex = 1; // Assuming the first row is the header
        foreach (ViewCustomTemplateExcelData data in excelData)
        {
            // Check for required fields
            if (string.IsNullOrWhiteSpace(data.GroupName))
                errorList.Add(new ErrorExcelData(rowIndex, TemplateColumn.GroupName, ErrorType.RequiredFieldMissing, data));

            if (string.IsNullOrWhiteSpace(data.PlanTemplateName))
                errorList.Add(new ErrorExcelData(rowIndex, TemplateColumn.PlanTemplateName, ErrorType.RequiredFieldMissing, data));

            if (string.IsNullOrWhiteSpace(data.PlanTemplateDetailTitle))
                errorList.Add(new ErrorExcelData(rowIndex, TemplateColumn.PlanTemplateDetailTitle, ErrorType.RequiredFieldMissing, data));

            // Additional validation for cycleType
            if (string.IsNullOrWhiteSpace(data.CycleType) || data.CycleType is not ("每月" or "每季" or "每年"))
            {
                errorList.Add(new ErrorExcelData(rowIndex, TemplateColumn.DefaultCycle, ErrorType.CycleFormatError, data));
            }

            // Validate multi-language Plan Template Name - all language versions are required
            if (string.IsNullOrWhiteSpace(data.PlanTemplateName))
                errorList.Add(new ErrorExcelData(rowIndex, TemplateColumn.PlanTemplateName, ErrorType.RequiredFieldMissing, data));

            if (string.IsNullOrWhiteSpace(data.PlanTemplateNameCh))
                errorList.Add(new ErrorExcelData(rowIndex, PlanTemplateNameColumn.Simplified, ErrorType.RequiredFieldMissing, data));

            if (string.IsNullOrWhiteSpace(data.PlanTemplateNameEn))
                errorList.Add(new ErrorExcelData(rowIndex, PlanTemplateNameColumn.English, ErrorType.RequiredFieldMissing, data));

            if (string.IsNullOrWhiteSpace(data.PlanTemplateNameJp))
                errorList.Add(new ErrorExcelData(rowIndex, PlanTemplateNameColumn.Japanese, ErrorType.RequiredFieldMissing, data));

            // Validate multi-language Plan Template Detail Title - all language versions are required
            if (string.IsNullOrWhiteSpace(data.PlanTemplateDetailTitle))
                errorList.Add(new ErrorExcelData(rowIndex, TemplateColumn.PlanTemplateDetailTitle, ErrorType.RequiredFieldMissing, data));

            if (string.IsNullOrWhiteSpace(data.PlanTemplateDetailTitleCh))
                errorList.Add(new ErrorExcelData(rowIndex, PlanTemplateDetailTitleColumn.Simplified, ErrorType.RequiredFieldMissing, data));

            if (string.IsNullOrWhiteSpace(data.PlanTemplateDetailTitleEn))
                errorList.Add(new ErrorExcelData(rowIndex, PlanTemplateDetailTitleColumn.English, ErrorType.RequiredFieldMissing, data));

            if (string.IsNullOrWhiteSpace(data.PlanTemplateDetailTitleJp))
                errorList.Add(new ErrorExcelData(rowIndex, PlanTemplateDetailTitleColumn.Japanese, ErrorType.RequiredFieldMissing, data));


            // Add formName validation if necessary
            Form? form = forms.FirstOrDefault(f => f.DisplayName == data.FormName);
            if (!string.IsNullOrWhiteSpace(data.FormName) && form is null)
            {
                errorList.Add(new ErrorExcelData(rowIndex, TemplateColumn.DocumentName, ErrorType.DocumentNameMismatch, data));
            }
            else
            {
                data.FormId = form!.Id!.Value;
            }

            rowIndex++;
        }

        return errorList;
    }
}

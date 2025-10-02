using AutoMapper;
using Base.Domain.Exceptions;
using Base.Infrastructure.Interface.Authentication;
using Main.Domain.AggregatesModel.CustomTemplateAggregate;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.Dto.ViewModel.CustomTemplate;
using Main.WebApi.Application.Commands.CustomTemplate;
using Main.WebApi.Application.Queries.CustomTemplate;
using Main.WebApi.Application.Queries.Plans;

namespace Main.WebApi.Controllers.v1.Plan;

public class CustomTemplateController(
    IUserService<Scuser> userService,
    ICustomPlanTemplateQuery customPlanTemplateQuery,
    ICustomRequestUnitQuery customRequestUnitQuery,
    ICustomPlanTemplateVersionQuery customPlanTemplateVersionQuery,
    IPlanIndicatorQuery planIndicatorQuery,
    IMapper mapper,
    IMediator mediator) : BaseController
{
    /// <summary>
    /// 查詢自訂指標計畫套版
    /// </summary>
    /// <param name="isDeployed">是否已發布</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <response code="200">成功查詢自訂指標計畫套版</response>
    /// <response code="400">查詢自訂指標計畫套版失敗</response>
    /// <response code="404">查無自訂指標計畫套版</response>
    /// <returns></returns>
    [HttpGet("")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    [ProducesResponseType(Status404NotFound)]
    public async Task<IActionResult> GetCustomPlanTemplate([FromBody] bool isDeployed, CancellationToken cancellationToken)
    {
        Scuser currentUser = await userService.Now(cancellationToken);
        IEnumerable<CustomPlanTemplate> result =
            await customPlanTemplateQuery.ListAsync(
                x => x.Version.Unit.TenantId == currentUser.CurrentTenant.TenantId && x.Version.IsDeployed == isDeployed,
                cancellationToken);
        return ActionResultBuilder(mapper.Map<IEnumerable<ViewCustomPlanTemplate>>(result));
    }


    /// <summary>
    /// 查詢自訂指標計畫套版
    /// </summary>
    /// <param name="templateVersionId">指標計畫樣版版本識別碼</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <response code="200">成功查詢自訂指標計畫套版</response>
    /// <response code="400">查詢自訂指標計畫套版失敗</response>
    /// <response code="404">查無自訂指標計畫套版</response>
    /// <returns></returns>
    [HttpGet("{templateVersionId}")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    [ProducesResponseType(Status404NotFound)]
    public async Task<IActionResult> GetDataByVersion(long templateVersionId, CancellationToken cancellationToken)
    {
        Scuser currentUser = await userService.Now(cancellationToken);
        CustomPlanTemplateVersion? result = await customPlanTemplateVersionQuery.GetByIdAsync(templateVersionId, currentUser.CurrentTenant.TenantId, cancellationToken);
        return ActionResultBuilder(result != null ? mapper.Map<ViewCustomPlanTemplateVersion>(result) : null);
    }

    /// <summary>
    /// 查詢要求單位
    /// </summary>
    /// <param name="cancellationToken">取消憑證</param>
    /// <response code="200">成功查詢要求單位</response>
    /// <response code="400">查詢要求單位失敗</response>
    /// <response code="404">查無要求單位</response>
    /// <returns></returns>
    [HttpGet("RequestUnits")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    [ProducesResponseType(Status404NotFound)]
    public async Task<IActionResult> QueryRequestUnits(CancellationToken cancellationToken)
    {
        Scuser currentUser = await userService.Now(cancellationToken);
        IEnumerable<CustomRequestUnit> result = await customRequestUnitQuery.ListAsync(currentUser.CurrentTenant.TenantId, cancellationToken);
        IEnumerable<long> versionIds = (await planIndicatorQuery.ListAsync(x => x.Plan!.TenantId == currentUser.CurrentTenant.TenantId, cancellationToken))
            .Where(x => x.VersionId.HasValue && x.RequestUnitId.HasValue)
            .Select(x => x.VersionId!.Value)
            .Distinct();

        result.ToList().ForEach(x => x.CustomPlanTemplateVersions.ToList().ForEach(v => v.CheckHasPlan(versionIds)));

        return ActionResultBuilder(mapper.Map<IEnumerable<ViewCustomRequestUnit>>(result));
    }

    /// <summary>
    /// 檢查要求單位是否已被使用
    /// </summary>
    /// <param name="requestUnitId">要求單位識別碼</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <response code="200">成功查詢要求單位是否已被使用</response>
    /// <returns></returns>
    [HttpGet("RequestUnit/{requestUnitId}/Check")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    public async Task<IActionResult> CheckRequestUnit([FromRoute] long requestUnitId, CancellationToken cancellationToken)
    {
        IEnumerable<PlanIndicator> result = await planIndicatorQuery.ListAsync(requestUnitId, cancellationToken);
        return ActionResultBuilder(result.Any());
    }

    /// <summary>
    /// 檢查要求單位套版版本是否已被使用
    /// </summary>
    /// <param name="requestUnitId">要求單位識別碼</param>
    /// <param name="versionId">版本識別碼</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <response code="200">成功查詢要求單位套版版本是否已被使用</response>
    /// <returns></returns>
    [HttpGet("RequestUnit/{requestUnitId}/Version/{versionId}/Check")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    public async Task<IActionResult> CheckCustomTemplateVersion([FromRoute] long requestUnitId, [FromRoute] long versionId, CancellationToken cancellationToken)
    {
        IEnumerable<PlanIndicator> result = await planIndicatorQuery.ListAsync(requestUnitId, versionId, cancellationToken);
        return ActionResultBuilder(result.Any());
    }

    /// <summary>
    /// 查詢自訂指標計畫要求單位的最新版本
    /// </summary>
    /// <param name="requestUnitId">要求單位識別碼</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <response code="200">成功查詢要求單位</response>
    /// <response code="400">查詢要求單位失敗</response>
    /// <response code="404">查無要求單位</response>
    /// <returns></returns>
    [HttpGet("RequestUnit/{requestUnitId}/LastVersion/Template")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    [ProducesResponseType(Status404NotFound)]
    public async Task<IActionResult> QueryRequestUnitsLastVersion(long requestUnitId, CancellationToken cancellationToken)
    {
        Scuser currentUser = await userService.Now(cancellationToken);
        IEnumerable<CustomPlanTemplate?> result =
            await customPlanTemplateQuery.GetLastVersionAsync(requestUnitId, currentUser.CurrentTenant.TenantId, cancellationToken);
        return ActionResultBuilder(result is null ? [] : mapper.Map<IEnumerable<ViewCustomPlanTemplate>>(result));
    }

    /// <summary>
    /// 新增要求單位
    /// </summary>
    /// <param name="command">新增要求單位命令</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <response code="200">成功寫入指派人</response>
    /// <response code="400">寫入指派人失敗</response>
    /// <returns></returns>
    [HttpPost("RequestUnit")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<IActionResult> CreateRequestUnit(CreateCustomRequestUnitCommand command, CancellationToken cancellationToken)
    {
        CustomRequestUnit result = await mediator.Send(command, cancellationToken);
        return ActionResultBuilder(mapper.Map<ViewCustomRequestUnit>(result));
    }

    /// <summary>
    /// 根據Excel資料重匯當前版本的CustomTemplates
    /// </summary>
    /// <param name="request">匯入自訂計畫範本命令</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <response code="200">成功寫入指派人</response>
    /// <response code="400">寫入指派人失敗</response>
    /// <returns></returns>
    [HttpPost("ExcelData")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<IActionResult> ImportPlanTemplateByExcelData([FromBody] ImportCustomTemplateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // if assign plan detail failed, return false
            Unit? result = await mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidException invalidException)
        {
            return ValidationProblem(invalidException.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// 發布自訂指標計畫樣版版本
    /// </summary>
    /// <param name="requestUnitId">要求單位識別碼</param>
    /// <param name="versionId">版本識別碼</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns></returns>
    [HttpPost("RequestUnit/{requestUnitId}/Version/{versionId}/Deploy")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    public async Task<IActionResult> DeployVersion([FromRoute] long requestUnitId, [FromRoute] long versionId, CancellationToken cancellationToken)
    {
        DeployCustomPlanTemplateVersionCommand command = new(requestUnitId, versionId);
        bool result = await mediator.Send(command, cancellationToken);
        return ActionResultBuilder(result);
    }

    /// <summary>
    /// 重新命名要求單位或版本
    /// </summary>
    /// <param name="command">重新命名要求單位或版本命令</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns></returns>
    [HttpPut("RequestUnit")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<IActionResult> RenameRequestUnit(RenameCustomRequestUnitCommand command, CancellationToken cancellationToken)
    {
        CustomRequestUnit result = await mediator.Send(command, cancellationToken);
        return ActionResultBuilder(result);
    }

    /// <summary>
    /// 刪除要求單位
    /// </summary>
    /// <param name="requestUnitId">要求單位識別碼</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns></returns>
    [HttpDelete("RequestUnit/{requestUnitId}")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<IActionResult> DeleteRequestUnit([FromRoute] long requestUnitId, CancellationToken cancellationToken)
    {
        DeleteCustomRequestUnitCommand command = new(requestUnitId);
        return ActionResultBuilder(await mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 刪除自訂計畫範本版本
    /// </summary>
    /// <param name="requestUnitId">要求單位識別碼</param>
    /// <param name="versionId">版本識別碼</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns></returns>
    [HttpDelete("RequestUnit/{requestUnitId}/Version/{versionId}")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<IActionResult> DeleteCustomTemplateVersion([FromRoute] long requestUnitId, [FromRoute] long versionId, CancellationToken cancellationToken)
    {
        DeleteCustomPlanTemplateVersionCommand command = new(requestUnitId, versionId);
        return ActionResultBuilder(await mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// 下載指標範本檔
    /// </summary>
    /// <response code="200">成功下載指標範本檔</response>
    /// <response code="404">找不到範本檔</response>
    /// <returns></returns>
    [HttpGet("Sample")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status404NotFound)]
    public IActionResult DownloadTemplate()
    {
        string relativePath = "Templates\\Excel"; // 專案相對路徑下的目錄
        string fileName = "自訂指標匯入版本檔.xlsx"; // 要使用的範本文件名

        // 組合完整路徑
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath, fileName);

        // 檢查檔案是否存在
        if (!System.IO.File.Exists(fullPath))
        {
            return NotFound("找不到指標範本檔");
        }

        // 讀取檔案內容並轉換為 base64
        byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);
        string base64String = Convert.ToBase64String(fileBytes);

        // 回傳包含檔案名稱和 base64 字串的物件
        var result = new
        {
            fileName,
            base64String
        };

        return Ok(result);
    }
}
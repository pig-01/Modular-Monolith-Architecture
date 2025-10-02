using AutoMapper;
using Base.Domain.Exceptions;
using Base.Infrastructure.Interface.Authentication;
using Main.Domain.AggregatesModel.PlanTemplateAggregate;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.Dto.ViewModel.PlanTemplate;
using Main.WebApi.Application.Commands.PlanTemplates;
using Main.WebApi.Application.Queries.PlanTemplates;

namespace Main.WebApi.Controllers.v1.Plan;

public class PlanTemplateController(
    IMapper mapper,
    IUserService<Scuser> userService,
    IMediator mediator,
    IPlanTemplateQuery planTemplateQuery) : BaseController
{

    /// <summary>
    /// 查詢計畫樣板
    /// </summary>
    /// <param name="indicatorIds">指標ID列表，以逗號分隔</param>
    /// <param name="planTemplateVersion">計畫樣板版本</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <response code="200">成功查詢計畫</response>
    /// <response code="400">查詢計畫失敗</response>
    /// <response code="404">查無計畫</response>
    /// <returns></returns>
    [HttpGet("")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    [ProducesResponseType(Status404NotFound)]
    public async Task<IActionResult> QueryPlanTemplate([FromQuery] string? indicatorIds, string? planTemplateVersion, CancellationToken cancellationToken)
    {
        Scuser currentUser = await userService.Now(cancellationToken);
        string[]? indicatorIdList = indicatorIds?.Split(",");
        IEnumerable<PlanTemplate> result = await planTemplateQuery.ListAsync(currentUser.CurrentTenant.TenantId, indicatorIdList, planTemplateVersion, cancellationToken);
        return ActionResultBuilder(mapper.Map<IEnumerable<ViewPlanTemplate>>(result, opt => opt.Items["Language"] = currentUser.CurrentCulture));
    }

    /// <summary>
    /// 查詢計畫樣板 By ID
    /// </summary>
    /// <param name="id">計畫樣板ID</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <response code="200">成功查詢計畫</response>
    /// <response code="400">查詢計畫失敗</response>
    /// <response code="404">查無計畫</response>
    /// <returns></returns>
    [HttpGet("{id}")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    [ProducesResponseType(Status404NotFound)]
    public async Task<IActionResult> QueryPlanTemplate([FromRoute] int id, CancellationToken cancellationToken)
    {
        Scuser currentUser = await userService.Now(cancellationToken);
        return ActionResultBuilder(mapper.Map<ViewPlanTemplate>(
            await planTemplateQuery.GetByIdAsync(id, cancellationToken), opt => opt.Items["Language"] = currentUser.CurrentCulture));
    }

    /// <summary>
    /// 取得版本清單
    /// </summary>
    /// <returns></returns>
    [HttpGet("Versions")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    [ProducesResponseType(Status404NotFound)]
    public async Task<IActionResult> QueryPlanTemplateVersions()
    {
        Scuser currentUser = await userService.Now();
        bool isAdmin = currentUser.IsSuperUser; // 假設 IsSuperUser 屬性表示使用者是否為管理員
        return ActionResultBuilder(await planTemplateQuery.GetVersionListAsync(isAdmin));
    }

    /// <summary>
    /// 同步計畫樣板的表單ID
    /// </summary>
    /// <param name="version"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{templateVersion}/Form/Sync")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    [ProducesResponseType(Status404NotFound)]
    public async Task<IActionResult> SyncPlanTemplateFormIds([FromRoute] string templateVersion, CancellationToken cancellationToken)
    {
        SyncPlanTemplateFormIdCommand command = new() { Version = templateVersion };
        bool result = await mediator.Send(command, cancellationToken);
        return ActionResultBuilder(result);
    }

    /// <summary>
    /// 取得準則清單
    /// </summary>
    /// <returns></returns>
    [HttpGet("GriRules")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    [ProducesResponseType(Status404NotFound)]
    public async Task<IActionResult> QueryGriRules(CancellationToken cancellationToken)
    {
        Scuser currentUser = await userService.Now(cancellationToken);
        IEnumerable<GriRule> griRules = await planTemplateQuery.GetGriRulesAsync(cancellationToken);
        return ActionResultBuilder(mapper.Map<IEnumerable<ViewGriRule>>(griRules, opt => opt.Items["Language"] = currentUser.CurrentCulture));
    }

    /// <summary>
    /// 取得當前版本的範本excel資料
    /// </summary>
    /// <param name="templateVersion">計畫樣板版本</param>
    /// <returns></returns>
    [HttpGet("ExcelData/{templateVersion}")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    [ProducesResponseType(Status404NotFound)]
    public async Task<IActionResult> QueryExcelData([FromRoute] string templateVersion) =>
        ActionResultBuilder(await planTemplateQuery.GetPlanTemplateExcelDataAsync(templateVersion));

    /// <summary>
    /// 根據Excel資料重匯當前版本的PlanTemplates
    /// </summary>
    /// <param name="request">附加表單ID命令</param>
    /// <response code="200">成功寫入指派人</response>
    /// <response code="400">寫入指派人失敗</response>
    /// <returns></returns>
    [HttpPost("ExcelData")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<IActionResult> ImportPlanTemplateByExcelData([FromBody] CreatePlanTemplateFromExcelCommand request)
    {
        try
        {
            // if assign plan detail failed, return false
            await mediator.Send(request);
        }
        catch (InvalidException invalidException)
        {
            return ValidationProblem(invalidException.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        return Ok();
    }

    /// <summary>
    /// 部署指定版本的計畫樣板
    /// </summary>
    /// <remarks>
    /// 將指定版本的所有計畫樣板的 IsDeploy 欄位設為 true
    /// </remarks>
    /// <param name="request">部署命令</param>
    /// <response code="200">成功部署計畫樣板</response>
    /// <response code="400">部署計畫樣板失敗</response>
    /// <returns></returns>
    [HttpPost("Deploy")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<IActionResult> DeployPlanTemplate([FromBody] DeployPlanTemplateCommand request)
    {
        try
        {
            await mediator.Send(request);
        }
        catch (InvalidException invalidException)
        {
            return ValidationProblem(invalidException.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        return Ok();
    }

    /// <summary>
    /// 下載指標範本檔
    /// </summary>
    /// <response code="200">成功下載指標範本檔</response>
    /// <response code="404">找不到範本檔</response>
    /// <returns></returns>
    [HttpGet("DownloadTemplate")]
    [Authorize(Policy = "User")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status404NotFound)]
    public IActionResult DownloadTemplate()
    {
        string relativePath = "wwwroot\\files\\ExcelTemplate"; // 專案相對路徑下的目錄
        string fileName = "指標匯入版本檔.xlsx"; // 要使用的範本文件名

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

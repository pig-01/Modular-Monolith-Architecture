using AutoMapper;
using Base.Infrastructure.Interface.Authentication;
using Main.Domain.AggregatesModel.PlanPermissionAggregate;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.Dto.ViewModel.PlanPermission;
using Main.WebApi.Application.Commands.PlanPermissions;
using MediatR;

namespace Main.WebApi.Controllers.v1.PlanPermissions;

[ApiController]
public class PlanPermissionController(
    ILogger<PlanPermissionController> logger,
    IPlanPermissionRepository planPermissionRepository,
    IUserService<Scuser> userService,
    IMediator mediator,
    IMapper mapper) : BaseController
{

    /// <summary>
    /// 查詢計畫權限清單
    /// </summary>
    /// <response code="200">成功查詢計畫</response>
    /// <response code="400">查詢計畫失敗</response>
    /// <response code="404">查無計畫</response>
    /// <returns></returns>
    [HttpGet("{planId}")]
    [ProducesResponseType(Status200OK)]
    public async Task<IActionResult> GetPlanPermissionByPlanId(string planId)
    {
        List<PlanPermission> permissions = await planPermissionRepository.GetAllByPlanIdAsync(int.Parse(planId));

        List<ViewPlanPermission> result = [];
        foreach (PlanPermission permission in permissions)
        {
            ViewPlanPermission viewPermission = mapper.Map<ViewPlanPermission>(permission);

            // 載入使用者資訊
            List<PlanPermissionRelatedItem> users = await planPermissionRepository.GetPlanPermissionUsersWithUserInfoAsync(permission.PlanPermissionID);
            viewPermission.PlanPermissionRelated = mapper.Map<ViewPlanPermissionRelatedItem[]>(users);

            result.Add(viewPermission);
        }

        return ActionResultBuilder(result);
    }

    /// <summary>
    /// 查詢計畫權限使用者清單
    /// </summary>
    /// <response code="200">成功查詢計畫</response>
    /// <response code="400">查詢計畫失敗</response>
    /// <response code="404">查無計畫</response>
    /// <returns></returns>
    [HttpGet("{id}/users")]
    public async Task<IActionResult> GetPlanPermissionUsers(int id)
    {
        List<PlanPermissionRelatedItem> planPermissionUsers = await planPermissionRepository.GetPlanPermissionUsersWithUserInfoAsync(id);
        List<ViewPlanPermissionRelatedItem> result = mapper.Map<List<ViewPlanPermissionRelatedItem>>(planPermissionUsers);
        return ActionResultBuilder(result);
    }

    /// <summary>
    /// 新增計畫權限
    /// </summary>
    /// <param name="command">包含多個權限類型的請求</param>
    /// <response code="200">成功新增計畫權限</response>
    /// <response code="400">新增計畫權限失敗</response>
    /// <returns>新增的權限ID清單</returns>
    [HttpPost("")]
    [ProducesResponseType(typeof(List<int>), Status200OK)]
    [ProducesResponseType(Status400BadRequest)]
    public async Task<IActionResult> CreatePlanPermissions([FromBody] CreatePlanPermissionsCommand command)
    {
        List<int> result = await mediator.Send(command);
        return Ok(result);
    }
}

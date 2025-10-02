using AutoMapper;
using Base.Infrastructure.Interface.Authentication;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.Dto.ViewModel.PlanSearch;
using Main.WebApi.Application.Commands.PlanSearch;
using Main.WebApi.Application.Queries.PlanSearch;

namespace Main.WebApi.Controllers.v1.PlanSearch
{
    public class PlanSearchController(
        IPlanSearchQuery planSearchQuery,
        IUserService<Scuser> userService,
        IMapper mapper,
        IMediator mediator,
        ILogger<PlanSearchController> logger
    ) : BaseController
    {

        [HttpGet("GetPlanSearch")]
        [Authorize(Policy = "User")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> GetPlanSearch([FromQuery] QueryPlanSearchRequest request)
        {
            Scuser scuser = await userService.Now();
            request.Responsible = scuser.UserId;
            request.TenantID = scuser.CurrentTenant.TenantId;
            List<ViewPlanSearchTreeData> result = await planSearchQuery.SearchPlansAsync(request);
            return ActionResultBuilder(result);
        }

        [HttpPost("InsertPlanSearchHistory")]
        [Authorize(Policy = "User")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task InsertPlanSearchHistory(CreatePlanSearchHistoryCommand request)
        {
            await mediator.Send(request);
        }


        [HttpGet("GetPlanSearchHistory")]
        [Authorize(Policy = "User")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IEnumerable<string>> GetPlanSearchHistory()
        {
            Scuser scuser = await userService.Now();
            string userId = scuser.UserId;
            string tenantId = scuser.CurrentTenant.TenantId;
            IEnumerable<string> result = await planSearchQuery.GetPlanSearchHistoriesAsync(userId, tenantId);
            return result;
        }

        [HttpGet("GetPlanSearchPredict")]
        [Authorize(Policy = "User")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IEnumerable<string>> GetPlanSearchPredict(string keyWord)
        {
            Scuser scuser = await userService.Now();
            string userId = scuser.UserId;
            string tenantId = scuser.CurrentTenant.TenantId;
            return await planSearchQuery.GetPlanSearchPredict(keyWord, tenantId);
        }

    }
}

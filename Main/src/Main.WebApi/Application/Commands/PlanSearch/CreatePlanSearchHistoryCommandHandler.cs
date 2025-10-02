
using Base.Infrastructure.Interface.Authentication;
using Base.Infrastructure.Interface.TimeZone;
using Main.Domain.AggregatesModel.PlanSearchAggreate;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.WebApi.Application.Queries.PlanSearch;

namespace Main.WebApi.Application.Commands.PlanSearch;

public class CreatePlanSearchHistoryCommandHandler(
    IPlanSearchRepository planSearchRepository,
    IPlanSearchQuery planSearchQuery,
    ITimeZoneService timeZoneService,
    IUserService<Scuser> userService
) : IRequestHandler<CreatePlanSearchHistoryCommand, Unit>
{
    public async Task<Unit> Handle(CreatePlanSearchHistoryCommand request, CancellationToken cancellationToken)
    {
        Scuser scuser = await userService.Now(cancellationToken);
        DateTime now = timeZoneService.Now;
        IEnumerable<string> existingData = await planSearchQuery.GetPlanSearchHistoriesAsync(scuser.UserId, scuser.CurrentTenant.TenantId);
        if (existingData.Count() >= 5)
        {
            await planSearchRepository.RemoveOlderPlanSearchHistory(scuser.UserId, scuser.CurrentTenant.TenantId);
        }
        await planSearchRepository.CreatePlanSearchHistoryAsync(request.KeyWord, scuser.UserId, now, scuser.UserId, now, scuser.UserId, scuser.CurrentTenant.TenantId);
        return Unit.Value;
    }
}

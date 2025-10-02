using Base.Infrastructure.Interface.Authentication;
using Base.Infrastructure.Interface.TimeZone;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.WebApi.Application.Queries.Plans;

namespace Main.WebApi.Application.Commands.Plans;

public class HideMultiplePlanCommandHandler(
    IPlanRepository planRepository,
    IPlanQuery planQuery,
    IUserService<Scuser> userService,
    ITimeZoneService timeZoneService
) : IRequestHandler<HideMultiplePlanCommand, Unit>
{

    [Authorize(Policy = "User")]
    public async Task<Unit> Handle(HideMultiplePlanCommand request, CancellationToken cancellationToken)
    {
        IEnumerable<Plan> plansToHide = (await planQuery.GetByIdsAsync(request.PlanIds, cancellationToken))
            .Where(p => p.Show);

        if (plansToHide.Any())
        {
            await planRepository.HideMultiplePlansAsync(
                plansToHide.Select(p => p.PlanId),
                timeZoneService.Now,
                userService.CurrentNow(cancellationToken).UserId,
                cancellationToken
            );
        }

        return Unit.Value;
    }
}

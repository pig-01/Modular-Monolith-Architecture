
using Base.Infrastructure.Interface.Authentication;
using Base.Infrastructure.Interface.TimeZone;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.AggregatesModel.UserAggregate;

namespace Main.WebApi.Application.Commands.Plans;

public class HidePlanCommandHandler(
    IPlanRepository planRepository,
    ITimeZoneService timeZoneService,
    IUserService<Scuser> userService
) : IRequestHandler<HidePlanCommand, Unit>
{

    [Authorize(Policy = "User")]
    public async Task<Unit> Handle(HidePlanCommand request, CancellationToken cancellationToken)
    {
        await planRepository.HidePlanAsync(
            request.PlanId,
            timeZoneService.Now,
            userService.CurrentNow(cancellationToken).UserId,
            cancellationToken
        );

        return Unit.Value;
    }

}

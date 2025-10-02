using Base.Infrastructure.Interface.Authentication;
using Base.Infrastructure.Interface.TimeZone;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.AggregatesModel.UserAggregate;

namespace Main.WebApi.Application.Commands.Plans;

/// <summary>
/// 刪除計畫
/// </summary>
public class CancelMultiplePlanCommandHandler(
    IUserService<Scuser> userService,
    ITimeZoneService timeZoneService,
    IPlanRepository planRepository) : IRequestHandler<CancelMultiplePlanCommand, Unit>
{
    [Authorize(Policy = "User")]
    public async Task<Unit> Handle(CancelMultiplePlanCommand request, CancellationToken cancellationToken)
    {
        foreach (int planId in request.PlanIds)
        {
            await planRepository.ArchivePlanAsync(
                planId,
                timeZoneService.Now,
                userService.CurrentNow(cancellationToken).UserId,
                cancellationToken);
        }
        return Unit.Value;
    }
}

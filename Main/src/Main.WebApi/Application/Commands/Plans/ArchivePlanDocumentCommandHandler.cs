using Base.Infrastructure.Interface.Authentication;
using Base.Infrastructure.Interface.TimeZone;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.AggregatesModel.UserAggregate;

namespace Main.WebApi.Application.Commands.Plans;

/// <summary>
/// 封存指標計畫文件處理
/// </summary>
public class ArchivePlanDocumentCommandHandler(
    ITimeZoneService timeZoneService,
    IUserService<Scuser> userService,
    IPlanRepository planRepository) : IRequestHandler<ArchivePlanDocumentCommand, Unit>
{
    [Authorize(Policy = "User")]
    public async Task<Unit> Handle(ArchivePlanDocumentCommand request, CancellationToken cancellationToken)
    {
        await planRepository.ArchivePlanDocumentAsync(
            request.PlanDocumentId,
            timeZoneService.Now,
            userService.CurrentNow(cancellationToken).UserId,
            cancellationToken);

        return Unit.Value;
    }
}

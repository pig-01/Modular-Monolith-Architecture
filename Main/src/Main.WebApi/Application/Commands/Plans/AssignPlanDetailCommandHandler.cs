using Base.Infrastructure.Interface.Authentication;
using Base.Infrastructure.Interface.TimeZone;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.WebApi.Application.Queries.Plans;

namespace Main.WebApi.Application.Commands.Plans;

public class AssignPlanDetailCommandHandler(
    ILogger<AssignPlanDetailCommandHandler> logger,
    IPlanDetailQuery planDetailQuery,
    IPlanRepository planRepository,
    IMediator mediator,
    ITimeZoneService timeZoneService,
    IUserService<Scuser> userService) : IRequestHandler<AssignPlanDetailCommand, Unit>
{
    [Authorize(Policy = "User")]
    public async Task<Unit> Handle(AssignPlanDetailCommand request, CancellationToken cancellationToken)
    {
        var planDetails = await planDetailQuery.GetByIdListAsync(request.PlanDetailIdList, cancellationToken);

        string modifiedUser = (await userService.Now(cancellationToken)).UserId;

        // 寫入指派人清單
        foreach (var item in planDetails)
        {
            item.Assign(request.ResponsibleList, modifiedUser, timeZoneService.Now);
            await planRepository.UpdatePlanDetailAsync(item, timeZoneService.Now, cancellationToken);
        }

        // 發通知信
        NotifyPlanDocumentCommand command = new()
        {
            PlanDetailIdList = request.PlanDetailIdList,
            PlanId = request.PlanId,
        };

        await mediator.Send(command, cancellationToken);

        return Unit.Value;
    }
}

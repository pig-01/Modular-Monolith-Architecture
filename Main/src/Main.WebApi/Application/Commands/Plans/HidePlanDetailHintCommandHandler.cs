using Base.Domain.Exceptions;
using Base.Infrastructure.Interface.Authentication;
using Base.Infrastructure.Interface.TimeZone;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.Dto.ViewModel.Plan;
using Main.WebApi.Application.Queries.Plans;
using Main.WebApi.Application.Queries.Plans.Impl;
using NPOI.SS.Formula.Functions;

namespace Main.WebApi.Application.Commands.Plans;

/// <summary>
/// 取消顯示hint
/// </summary>
public class HidePlanDetailHintCommandHandler(
    ITimeZoneService timeZoneService,
    IUserService<Scuser> userService,
    IPlanDetailQuery planDetailQuery,
    IPlanRepository planRepository) : IRequestHandler<HidePlanDetailHintCommand, Unit>
{
    [Authorize(Policy = "User")]
    public async Task<Unit> Handle(HidePlanDetailHintCommand request, CancellationToken cancellationToken)
    {

        string userId = userService.CurrentNow(cancellationToken).UserId;
        // 先找到對應的 planDetail
        ViewPlanDetail planDetail = await planDetailQuery.GetDtoByIdAsync(request.PlanDetailId, userId, cancellationToken) ??
            throw new NotFoundException($"PlanDetail with ID {request.PlanDetailId} not found.");

        // 取消顯示hint
        await planRepository.HidePlanDetailHintAsync(
            userId,
            timeZoneService.Now,
            request.PlanDetailId,
            cancellationToken
        );

        return Unit.Value;
    }
}

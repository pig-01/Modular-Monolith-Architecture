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
/// 取消串接netzero
/// </summary>
public class CancelConnectNetZeroToPlanDetailCommandHandler(
    ITimeZoneService timeZoneService,
    IUserService<Scuser> userService,
    IPlanDetailQuery planDetailQuery,
    IMediator mediator,
    IPlanRepository planRepository) : IRequestHandler<CancelConnectNetZeroToPlanDetailCommand, Unit>
{
    [Authorize(Policy = "User")]
    public async Task<Unit> Handle(CancelConnectNetZeroToPlanDetailCommand request, CancellationToken cancellationToken)
    {

        string userId = userService.CurrentNow(cancellationToken).UserId;
        // 先找到對應的 planDetail
        ViewPlanDetail planDetail = await planDetailQuery.GetDtoByIdAsync(request.PlanDetailId, userId, cancellationToken) ??
            throw new NotFoundException($"PlanDetail with ID {request.PlanDetailId} not found.");

        // 取消串接netZero資料
        await planRepository.CancelConnectNetZeroToPlanDetailAsync(
            userId,
            timeZoneService.Now,
            request.PlanDetailId,
            cancellationToken
        );

        // 把當前週期換成年
        ModifyPlanDetailCycleCommand modifyPlanDetailCycleCommand = new()
        {
            PlanDetailId = planDetail.PlanDetailId,
            CycleType = "year",
            EndDate = (DateTime)planDetail.EndDate,
            PlanDocumentCycleArray = [
                new() {
                    StartDate = new DateTime((int) planDetail.Year, 1, 1, 0, 0, 0),
                    EndDate = (DateTime)planDetail.EndDate,
                    Year = planDetail.Year
                    }
            ]
        };

        await mediator.Send(modifyPlanDetailCycleCommand, cancellationToken);

        return Unit.Value;
    }
}

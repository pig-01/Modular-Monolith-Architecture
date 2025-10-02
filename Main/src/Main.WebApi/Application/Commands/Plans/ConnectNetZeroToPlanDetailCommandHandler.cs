using Base.Domain.Exceptions;
using Base.Infrastructure.Interface.Authentication;
using Base.Infrastructure.Interface.TimeZone;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.Dto.ViewModel.Plan;
using Main.WebApi.Application.Queries.Plans;

namespace Main.WebApi.Application.Commands.Plans;

/// <summary>
/// 串接netzero
/// </summary>
public class ConnectNetZeroToPlanDetailCommandHandler(
    ITimeZoneService timeZoneService,
    IUserService<Scuser> userService,
    IPlanDetailQuery planDetailQuery,
    IPlanFactoryQuery planFactoryQuery,
    IMediator mediator,
    IPlanRepository planRepository) : IRequestHandler<ConnectNetZeroToPlanDetailCommand, Unit>
{
    [Authorize(Policy = "User")]
    public async Task<Unit> Handle(ConnectNetZeroToPlanDetailCommand request, CancellationToken cancellationToken)
    {

        // 先找到對應的 planDetail
        string userId = userService.CurrentNow(cancellationToken).UserId;
        string tenantId = userService.CurrentNow(cancellationToken).CurrentTenant.TenantId;
        ViewPlanDetail planDetail = await planDetailQuery.GetDtoByIdAsync(request.PlanDetailId, userId, cancellationToken) ??
            throw new NotFoundException($"PlanDetail with ID {request.PlanDetailId} not found.");


        // 寫入netZero資料
        await planRepository.ConnectNetZeroToPlanDetailAsync(
            request.ApiConnectionId,
            request.NetZeroReportId,
            request.NetZeroReportName,
            userId,
            timeZoneService.Now,
            request.PlanDetailId,
            cancellationToken
        );

        // 週期轉換成年 + 把原本的PlanDocument都移到封存
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


        //formstats
        //得到Plan的FactoryID 迴圈建立多筆資料。
        IEnumerable<ViewPlanAreaData> factoryList = await planFactoryQuery.GetPlanAreaDataByPlanDetailId(request.PlanDetailId, tenantId, cancellationToken);
        foreach (ViewPlanAreaData factory in factoryList)
        {
            // 建一筆新的週期是年的planDocument
            PlanDocument newPlanDocument = new(
                request.PlanDetailId,
                new DateTime((int)planDetail.Year, 1, 1, 0, 0, 0),
                (DateTime)planDetail.EndDate,
                factory.PlanFactoryId,
                planDetail.Year,
                null,
                null,
                userId,
                userId
            )
            {
                FormStatus = "5"
            };

            await planRepository.AddPlanDocumentAsync(newPlanDocument, cancellationToken);
        }

        // 跑一次資料直接抓取的api資料
        ModifyPlanDocumentDataByNetZeroCommand command = new()
        {
            PlanDetailId = request.PlanDetailId
        };

        await mediator.Send(command, cancellationToken);

        return Unit.Value;
    }
}

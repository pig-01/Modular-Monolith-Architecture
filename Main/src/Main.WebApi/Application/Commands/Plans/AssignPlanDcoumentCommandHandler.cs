using Base.Domain.Exceptions;
using Base.Infrastructure.Interface.Authentication;
using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.WebApi.Application.Queries.Plans;
using Main.WebApi.Application.Queries.Users;

namespace Main.WebApi.Application.Commands.Plans;

public class AssignPlanDcoumentCommandHandler(
    ILogger<AssignPlanDcoumentCommandHandler> logger,
    IUserQuery userQuery,
    IPlanQuery planQuery,
    IPlanRepository planRepository,
    IUserService<Scuser> userService) : IRequestHandler<AssignPlanDocumentCommand, Unit>
{

    //註解整個方法，因為這個沒在用了，但裡面有一些方法參數有新增，會導致報錯。
    [Authorize(Policy = "User")]
    public async Task<Unit> Handle(AssignPlanDocumentCommand request, CancellationToken cancellationToken)
    {
        // // 檢查計畫是否存在
        // Plan plan = await planQuery.GetByIdAsync(request.PlanId, cancellationToken) ?? throw new NotFoundException("Plan is not found");
        //
        // // 檢查負責人是否存在
        // Scuser responsible = await userQuery.GetByIdAsync(request.ResponsiblePerson, cancellationToken) ?? throw new NotFoundException("Responsible person is not found");
        //
        // // 檢查被指派的明細是否都是存在的
        // HashSet<int> planDetailIdSet = [.. plan.PlanDetails.Select(x => x.PlanDetailId)];
        // if (!request.DataList.All(item => planDetailIdSet.Contains(item.PlanDetailId))) throw new HandleException("Some details are not included in the plan");
        //
        // // 設定登入人為 指派人 與 建立人 兩種角色
        // string createdUser = (await userService.Now(cancellationToken)).UserId;
        //
        // // 替換指派清單為表單中的資料，並轉換為計畫文件
        // IEnumerable<PlanDocument> planDocuments = request.DataList.Select(assignPlanDetail =>
        // {
        //     DateTime startDate = DateTime.TryParse(assignPlanDetail.StartDate, out DateTime sDate) ? sDate : throw new ParameterException("StartDate Parse Failed");
        //     DateTime endDate = DateTime.TryParse(assignPlanDetail.EndDate, out DateTime eDate) ? eDate : throw new ParameterException("EndDate Parse Failed");
        //
        //     return new PlanDocument(assignPlanDetail.PlanDetailId, startDate, endDate, assignPlanDetail.Year, assignPlanDetail.Quarter, assignPlanDetail.Month, createdUser, createdUser);
        // });
        //
        // await planRepository.AssignPlanDocumentAsync(plan.PlanId, responsible.UserId, createdUser, planDocuments, cancellationToken);
        //
        // // 指派指標計畫文件給負責人
        // plan.Assign(plan.PlanName, responsible, await userService.Now(cancellationToken), planDocuments);

        return Unit.Value;
    }
}

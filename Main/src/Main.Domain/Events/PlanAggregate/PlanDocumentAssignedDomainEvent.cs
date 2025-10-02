using Main.Domain.AggregatesModel.PlanAggregate;
using Main.Domain.AggregatesModel.UserAggregate;
using MediatR;

namespace Main.Domain.Events.PlanAggregate;

public class PlanDocumentAssignedDomainEvent(string planName, Scuser responsible, Scuser assign, IEnumerable<PlanDetail> planDetails) : INotification
{
    /// <summary>
    /// 計畫名稱
    /// </summary>
    public string PlanName { get; } = planName;

    /// <summary>
    /// 負責人
    /// </summary>
    public Scuser Responsible { get; } = responsible;

    /// <summary>
    /// 指派人
    /// </summary>
    public Scuser Assign { get; } = assign;

    /// <summary>
    /// 指標計畫明細清單
    /// </summary>
    public IEnumerable<PlanDetail> PlanDetails { get; } = planDetails;
}

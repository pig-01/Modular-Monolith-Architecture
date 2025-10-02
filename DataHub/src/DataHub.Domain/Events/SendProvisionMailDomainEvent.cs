using DataHub.Domain.AggregatesModel.TenantAggregate;
using MediatR;

namespace DataHub.Domain.Events;

public class SendProvisionMailDomainEvent(Tenant tenant) : INotification
{
    /// <summary>
    /// 租戶
    /// </summary>
    public Tenant Tenant { get; } = tenant;
}

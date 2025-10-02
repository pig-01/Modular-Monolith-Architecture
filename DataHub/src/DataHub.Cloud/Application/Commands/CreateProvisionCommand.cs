using DataHub.Cloud.Models.Provision;
using DataHub.Domain.AggregatesModel.OrderAggregate;
using MediatR;

namespace DataHub.Cloud.Application.Commands;

public class CreateProvisionCommand(Customer customer, string? tenantName, string sku, string expirationDate) : IRequest<Order>
{
}
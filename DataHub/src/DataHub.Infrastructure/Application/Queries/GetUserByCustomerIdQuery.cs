using System.Text.Json.Serialization;
using Base.Domain.SeedWorks.MediatR;
using DataHub.Domain.AggregatesModel.UserAggregate;

namespace DataHub.Infrastructure.Application.Queries;

public class GetUserByCustomerIdQuery(string customerId) : IQuery<Scuser>
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("customerId")]
    public string CustomerId { get; set; } = customerId;
}

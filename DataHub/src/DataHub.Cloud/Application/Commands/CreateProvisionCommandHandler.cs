using Base.Infrastructure.Interface.TimeZone;
using Base.Infrastructure.Toolkits.Extensions;
using DataHub.Cloud.Repositories;
using DataHub.Domain.AggregatesModel.OrderAggregate;
using MediatR;

namespace DataHub.Cloud.Application.Commands;

public class CreateProvisionCommandHandler(
    IProvisionRepository provisionRepository,
    ITimeZoneService timeZoneService) : IRequestHandler<CreateProvisionCommand, Order>
{
    public async Task<Order> Handle(CreateProvisionCommand command, CancellationToken cancellationToken)
    {
        throw new Exception();
    }

    /// <summary>
    /// 取得Pricing明細資訊
    /// </summary>
    /// <param name="pricingDetails">資費方案明細</param>
    /// <param name="pricingCode">資費方案代號</param>
    /// <returns></returns>
    private static int GetPricingDetailValue(ICollection<PricingDetail> pricingDetails, string pricingCode)
    {
        PricingDetail? detail = pricingDetails.FirstOrDefault(x => x.PricingDetailCode == pricingCode);
        if (detail is null) return 0;
        if (int.TryParse(detail.PricingDetailValue, out int result))
        {
            return result;
        }
        return 0;
    }
}

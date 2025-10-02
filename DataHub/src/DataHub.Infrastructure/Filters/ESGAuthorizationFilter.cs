using Base.Infrastructure.Toolkits.Extensions;
using DataHub.Infrastructure.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DataHub.Infrastructure.Filters;

public class DemoAuthorizationFilter(ILogger<DemoAuthorizationFilter> logger, IOptions<RadarOptions> options) : IEndpointFilter
{

    public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        ValidateRequestIP(context.HttpContext);

        return next(context);
    }

    /// <summary>
    /// 檢查來源IP位置是否允許
    /// </summary>
    /// <param name="context"></param>
    public void ValidateRequestIP(HttpContext context)
    {
        string clientIP = context.GetClientIp();
        logger.LogInformation("DemoAuthorizationFilter IP: {ip}", clientIP);

        string[] localhosts = new string[] { "::1", "127.0.0.1" };
        if (localhosts.Contains(clientIP)) { return; }

        List<string>? allowIPs = options.Value.ValidateIp;
        if (allowIPs is null) return;
        foreach (string validIpMaskRule in allowIPs)
        {
            if (validIpMaskRule.IpCompare(clientIP)) { return; }
        }

        throw new UnauthorizedAccessException("IP位址異常");
    }
}

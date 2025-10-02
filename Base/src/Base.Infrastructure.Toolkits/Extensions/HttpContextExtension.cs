using Microsoft.AspNetCore.Http;

namespace Base.Infrastructure.Toolkits.Extensions;

public static class HttpContextExtension
{
    /// <summary>
    /// 取得Header
    /// </summary>
    /// <param name="context"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string GetHeader(this HttpContext context, string key) => context.Request.Headers[key].ToString();

    /// <summary>
    /// 取得ClientIP
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static string GetClientIp(this HttpContext context)
    {
        const string HTTP_CONTEXT = "MS_HttpContext";
        const string REMOTE_ENDPOINT_MESSAGE = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";
        const string OWIN_CONTEXT = "MS_OwinContext";

        if (context.Items.ContainsKey(HTTP_CONTEXT))
        {
            dynamic? ctx = context.Items[HTTP_CONTEXT];
            if (ctx != null)
            {
                return ctx.Request.UserHostAddress;
            }
        }

        //Self-hosting
        if (context.Items.ContainsKey(REMOTE_ENDPOINT_MESSAGE))
        {
            dynamic? remoteEndpoint = context.Items[REMOTE_ENDPOINT_MESSAGE];
            if (remoteEndpoint != null)
            {
                return remoteEndpoint.Address;
            }
        }

        //Owin-hosting
        if (context.Items.ContainsKey(OWIN_CONTEXT))
        {
            dynamic? ctx = context.Items[OWIN_CONTEXT];
            if (ctx != null)
            {
                return ctx.Request.RemoteIpAddress;
            }
        }

        if (context.Connection.RemoteIpAddress != null)
        {
            return context.Connection.RemoteIpAddress.ToString();
        }

        // Always return all zeroes for any failure
        return "0.0.0.0";
    }
}

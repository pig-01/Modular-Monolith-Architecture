using System.Net.Http.Headers;
using Main.Infrastructure.Options.Bizform;
using Main.Infrastructure.Options.NetZero;

namespace Main.WebApi.Extensions;

public static class HttpExtension
{
    public static void AddSystemHttpClient(this WebApplicationBuilder builder)
    {
        // get the BizformOption from the configuration
        BizformOption bizformOption =
            builder.Configuration
                .GetSection(BizformOption.Position)
                .Get<BizformOption>() ?? throw new ArgumentNullException(null, nameof(BizformOption));

        builder.Services.AddHttpClient("Bizform", httpClient =>
        {
            httpClient.BaseAddress = new Uri(bizformOption.Url);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(bizformOption.Authorization.Scheme, bizformOption.Authorization.Token);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("User-Agent", "demoDemo");
        })
        .ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        });

        builder.Services.AddHttpClient("Bizform_form", httpClient =>
        {
            httpClient.BaseAddress = new Uri(bizformOption.Url);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(bizformOption.Authorization.Scheme, bizformOption.Authorization.Token);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            httpClient.DefaultRequestHeaders.Add("User-Agent", "demoDemo");
        })
        .ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        });


        // get the NetZeroOption from the configuration
        NetZeroOption netZeroOption =
            builder.Configuration
                .GetSection(NetZeroOption.Position)
                .Get<NetZeroOption>() ?? throw new ArgumentNullException(null, nameof(NetZeroOption));

        builder.Services.AddHttpClient("NetZero", httpClient =>
        {
            httpClient.BaseAddress = new Uri(netZeroOption.Url);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("User-Agent", "demoDemo");
        })
        .ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        });
    }
}

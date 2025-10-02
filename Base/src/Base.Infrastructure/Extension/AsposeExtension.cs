using Base.Aspose;
using Base.Domain.Exceptions;
using Base.Domain.Options.Aspose;
using Base.Infrastructure.Toolkits.Extensions;
using Base.Infrastructure.Toolkits.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Base.Infrastructure.Extension;

public static class AsposeExtension
{
    public static IServiceCollection AddAsposeService(this IServiceCollection services) => services.AddScoped<IAsposeFactory, AsposeFactory>();

    public static void LoadLicense(IConfiguration configuration)
    {
        AsposeOption option = configuration
            .GetSection(AsposeOption.Position)
            .Get<AsposeOption>()
            .IsNotNull(new ConfigNullException(MessageResource.ConfigNullExceptionMessage.SetCustomerMessage("Aspose設定異常")));

        AsposeCore.SetLicense(option.LicensePosition.IsNotNull(new ConfigNullException(MessageResource.ConfigNullExceptionMessage.SetCustomerMessage("Aspose設定異常"))));
    }
}
using System.Reflection;
using Autofac;
using Base.Domain.Exceptions;
using Base.Domain.Options.Aspose;
using Base.Domain.SeedWorks;
using Base.Infrastructure.Interface;
using Base.Infrastructure.Toolkits.Extensions;
using Base.Infrastructure.Toolkits.Resources;
using Microsoft.Extensions.Configuration;

namespace Base.Aspose.Module;

public class AsposeModule(IConfiguration configuration) : BaseModule(configuration), IModuleInitializer
{
    private readonly IConfiguration configuration = configuration;

    public override string ModuleName => nameof(AsposeModule);

    public string ModuleVersion => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";

    protected override void Load(ContainerBuilder builder) =>
        // 加入Aspose工廠服務
        builder.RegisterType<AsposeFactory>()
            .As<IAsposeFactory>()
            .InstancePerLifetimeScope()
            .AutoActivate();

    public override void Initialize()
    {
        // 讀取Aspose授權檔案
        AsposeOption option = configuration
            .GetSection(AsposeOption.Position)
            .Get<AsposeOption>()
            .IsNotNull(new ConfigNullException(MessageResource.ConfigNullExceptionMessage.SetCustomerMessage("Aspose設定異常")));

        AsposeCore.SetLicense(option.LicensePosition.IsNotNull(new ConfigNullException(MessageResource.ConfigNullExceptionMessage.SetCustomerMessage("Aspose設定異常"))));
    }

}

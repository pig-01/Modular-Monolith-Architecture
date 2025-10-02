using System.Reflection;
using Autofac;
using Base.Domain.Exceptions;
using Base.Domain.Options.Cryptography;
using Base.Domain.SeedWorks;
using Base.Infrastructure.Interface;
using Base.Infrastructure.Interface.Security.Cryptography;
using Base.Infrastructure.Toolkits.Extensions;
using Base.Security.Cryptography;
using Microsoft.Extensions.Configuration;

namespace Base.Security.Module;

public class SecurityModule(IConfiguration configuration) : BaseModule(configuration), IModuleInitializer
{
    private readonly IConfiguration configuration = configuration;

    public override string ModuleName => nameof(SecurityModule);

    public string ModuleVersion => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";

    protected override void Load(ContainerBuilder builder)
    {
        // 取得加密設定
        CryptographyOption cryptographyOption = configuration
            .GetSection(CryptographyOption.Position)
            .Get<CryptographyOption>()
            .IsNotNull(new ConfigNullException("加密設定異常"))!;

        // 加入服務
        builder.Register<IAsymmetricAlgorithmService>(
            context => new AsymmetricAlgorithmService(cryptographyOption.AsymmetricAlgorithmSetting))
            .InstancePerLifetimeScope();

        builder.Register<ISymmetricAlgorithmService>(
            context => new SymmetricAlgorithmService(cryptographyOption.SymmetricAlgorithmSetting))
            .InstancePerLifetimeScope();

        builder.Register<IHashAlgorithmService>(
            context => new HashAlgorithmService(cryptographyOption.HashAlgorithmSetting))
            .InstancePerLifetimeScope();
    }
}

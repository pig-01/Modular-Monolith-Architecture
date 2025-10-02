using Base.Domain.Exceptions;
using Base.Domain.Options.Cryptography;
using Base.Infrastructure.Interface.Security.Cryptography;
using Base.Infrastructure.Toolkits.Extensions;
using Base.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Base.Infrastructure.Extension;

public static class SecurityExtension
{
    /// <summary>
    /// 加入安全服務
    /// </summary>
    /// <param name="services">服務集合</param>
    /// <param name="configuration">設定</param>
    /// <returns>服務集合</returns>
    public static IServiceCollection AddSecurityService(this IServiceCollection services, IConfiguration configuration)
    {
        // 取得加密設定
        IConfigurationSection configurationSection = configuration.GetSection(CryptographyOption.Position);
        services.Configure<CryptographyOption>(configurationSection);
        CryptographyOption cryptographyOption = configurationSection.Get<CryptographyOption>().IsNotNull(new ConfigNullException("加密設定異常"))!;

        // 加入服務
        services.AddScoped<IAsymmetricAlgorithmService, AsymmetricAlgorithmService>(serviceProvider =>
        {
            IOptions<CryptographyOption> option = serviceProvider.GetRequiredService<IOptions<CryptographyOption>>();
            return new AsymmetricAlgorithmService(option.Value.AsymmetricAlgorithmSetting);
        });
        services.AddScoped<ISymmetricAlgorithmService, SymmetricAlgorithmService>(serviceProvider =>
        {
            IOptions<CryptographyOption> option = serviceProvider.GetRequiredService<IOptions<CryptographyOption>>();
            return new SymmetricAlgorithmService(option.Value.SymmetricAlgorithmSetting);
        });
        services.AddScoped<IHashAlgorithmService, HashAlgorithmService>(serviceProvider =>
        {
            IOptions<CryptographyOption> option = serviceProvider.GetRequiredService<IOptions<CryptographyOption>>();
            return new HashAlgorithmService(option.Value.HashAlgorithmSetting);
        });
        return services;
    }

    public static string EncryptString(this string input)
    {
        CryptographyOption option = new();
        return new SymmetricAlgorithmService(option.SymmetricAlgorithmSetting).EncryptWithKey(input);
    }

    public static string DecryptString(this string input)
    {
        CryptographyOption option = new();
        return new SymmetricAlgorithmService(option.SymmetricAlgorithmSetting).DecryptWithKey(input);
    }
}

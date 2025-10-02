using Base.Domain.Exceptions;
using Base.Domain.Options.FileService;
using Base.Files.Local;
using Base.Files.Remote;
using Base.Infrastructure.Interface.Files;
using Base.Infrastructure.Toolkits.Extensions;
using Base.Infrastructure.Toolkits.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static Base.Domain.Enums.StorageModeEnum;

namespace Base.Infrastructure.Extension;

public static class FileServiceExtension
{
    // public static IServiceCollection AddFileService(this IServiceCollection services, IConfiguration configuration)
    // {
    //     // 取得加密設定
    //     IConfigurationSection configurationSection = configuration.GetSection(FileServiceOption.Position);
    //     services.Configure<FileServiceOption>(configurationSection);
    //     FileServiceOption fileServiceOption =
    //         configurationSection
    //             .Get<FileServiceOption>()
    //             .IsNotNull(new ConfigNullException(MessageResource.ConfigNullExceptionMessage.SetCustomerMessage("檔案服務設定異常")));
    //
    //     return services.AddScoped<IFileService>(serviceProvider => fileServiceOption.Mode switch
    //         {
    //             StorageMode.Local => new LocalFileService(fileServiceOption.LocalStorageSetting),
    //             StorageMode.Azure => new AzureFileService(fileServiceOption.AzureStorageSetting),
    //             _ => throw new ConfigNullException(MessageResource.ConfigNullExceptionMessage.SetCustomerMessage("檔案服務設定異常"))
    //         });
    // }
}

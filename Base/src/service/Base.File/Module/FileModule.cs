using System.Reflection;
using Autofac;
using Base.Domain.Exceptions;
using Base.Domain.Options.FileService;
using Base.Domain.SeedWorks;
using Base.Files.Adapter;
using Base.Files.Helper;
using Base.Files.Local;
using Base.Files.Remote;
using Base.Infrastructure.Interface;
using Base.Infrastructure.Interface.Files;
using Base.Infrastructure.Toolkits.Extensions;
using Base.Infrastructure.Toolkits.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static Base.Domain.Enums.StorageModeEnum;

namespace Base.Files.Module;

public class FileModule(IConfiguration configuration) : BaseModule(configuration), IModuleInitializer
{
    private readonly IConfiguration configuration = configuration;

    public override string ModuleName => nameof(FileModule);

    public string ModuleVersion => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";

    protected override void Load(ContainerBuilder builder)
    {
        // 取得檔案服務設定
        FileServiceOption fileServiceOption = configuration
            .GetSection(FileServiceOption.Position)
            .Get<FileServiceOption>()
            .IsNotNull(new ConfigNullException(MessageResource.ConfigNullExceptionMessage.SetCustomerMessage("檔案服務設定異常")));

        // 註冊檔案類型檢測服務
        builder.RegisterType<MimeDetectiveFileTypeDetector>()
            .As<IFileTypeDetector>()
            .SingleInstance();

        // Switch Statement 註冊
        RegisterComponents(fileServiceOption.Mode);

        void RegisterComponents(StorageMode mode)
        {
            switch (mode)
            {
                case StorageMode.Local:
                    RegisterLocalComponents();
                    break;
                case StorageMode.Azure:
                    RegisterAzureComponents();
                    break;
                default:
                    throw new ConfigNullException(MessageResource.ConfigNullExceptionMessage.SetCustomerMessage("不支援的儲存模式"));
            }
        }

        void RegisterLocalComponents()
        {
            builder.RegisterType<LocalStorageAdapter>()
                .As<IStorageAdapter>()
                .WithParameter("settings", fileServiceOption.LocalStorageSetting)
                .InstancePerLifetimeScope();

            builder.RegisterType<LocalFileService>()
                .As<IFileService>()
                .WithParameter("settings", fileServiceOption.LocalStorageSetting)
                .InstancePerLifetimeScope();
        }
        void RegisterAzureComponents()
        {
            builder.RegisterType<AzureStorageAdapter>()
                .As<IStorageAdapter>()
                .WithParameter("settings", fileServiceOption.AzureStorageSetting)
                .InstancePerLifetimeScope();

            builder.RegisterType<AzureFileService>()
                .As<IFileService>()
                .WithParameter("settings", fileServiceOption.AzureStorageSetting)
                .InstancePerLifetimeScope();
        }
    }
}

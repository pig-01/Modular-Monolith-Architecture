using Base.Aspose.Module;
using Base.Files.Module;
using Base.Infrastructure.Extension;
using Base.Security.Module;

namespace Main.WebApi.Extensions;

/// <summary>
/// 擴展基底
/// </summary>
public static class BaseExtension
{
    public static void AddBaseService(this WebApplicationBuilder builder)
    {
        // 註冊核心服務
        builder.Services.AddBaseCore();

        // 設定 Autofac 容器
        //builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
        //{
        //    // 註冊所有模組
        //    containerBuilder.RegisterModules(
        //        builder.Configuration,
        //        typeof(SecurityModule).Assembly,
        //        typeof(FileModule).Assembly
        //    );
        //});

        IConfiguration configuration = builder.Configuration;

        builder.Host.ConfigureContainer<ContainerBuilder>(builder => builder.RegisterModule(new AsposeModule(configuration)));
        builder.Host.ConfigureContainer<ContainerBuilder>(builder => builder.RegisterModule(new SecurityModule(configuration)));
        builder.Host.ConfigureContainer<ContainerBuilder>(builder => builder.RegisterModule(new FileModule(configuration)));
    }
}

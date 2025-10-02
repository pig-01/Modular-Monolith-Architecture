using Autofac;
using Autofac.Extensions.DependencyInjection;
using Scheduler.Registers;

namespace Scheduler.Extensions;

/// <summary>
/// 擴展 Autofac 模組
/// </summary>
public static class ModuleExtension
{
    /// <summary>
    /// 擴展 Autofac 模組
    /// </summary>
    /// <param name="builder"></param>
    public static void AddApplicationServiceWithAutofac(this WebApplicationBuilder builder)
    {
        // 使用 Autofac 作為 DI 容器
        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

        // 註冊 Autofac 模組
        builder.Host.ConfigureContainer<ContainerBuilder>(builder => builder.RegisterModule(new QueryRegisterModule()));
        builder.Host.ConfigureContainer<ContainerBuilder>(builder => builder.RegisterModule(new RepositoryRegisterModule()));
        builder.Host.ConfigureContainer<ContainerBuilder>(builder => builder.RegisterModule(new ServiceRegisterModule()));
    }
}

using System.Reflection;
using Autofac;
using Base.Domain.SeedWorks;
using Base.Infrastructure.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Base.Infrastructure.Extension;

public static class ServiceCollectionExtensions
{
    // 註冊核心服務
    public static IServiceCollection AddBaseCore(this IServiceCollection services) =>
        // 這裡註冊一些核心服務，如果有的話
        services;

    // 配置 Autofac 容器並註冊模組
    public static void RegisterModules(this ContainerBuilder builder, IConfiguration configuration, params Assembly[] assemblies)
    {
        // 註冊服務工廠
        builder.RegisterType<ModuleFactory>().As<IModuleFactory>().SingleInstance();

        // 從程序集中查找並註冊所有繼承了 BaseModule 的模組
        foreach (Assembly assembly in assemblies)
        {
            List<Type> moduleTypes = assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(BaseModule)) && !t.IsAbstract)
                .ToList();

            foreach (Type? moduleType in moduleTypes)
            {
                // 建立模組實例
                BaseModule? moduleInstance = (BaseModule)Activator.CreateInstance(
                    moduleType,
                    new object[] { configuration }
                );

                // 註冊模組
                builder.RegisterModule(moduleInstance);

                // 註冊模組本身，以便可以注入它
                builder.RegisterInstance(moduleInstance)
                    .As(moduleType)
                    .SingleInstance();
            }
        }
    }
}
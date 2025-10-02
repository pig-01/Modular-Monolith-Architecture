using System.Reflection;

namespace Main.WebApi.Registers;

public class RepositoryRegisterModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder) => builder.RegisterAssemblyTypes(Assembly.Load("Main.Repository"))
           .Where(t => t.Name.EndsWith("Repository", StringComparison.OrdinalIgnoreCase))
           .AsImplementedInterfaces()
           .InstancePerLifetimeScope();
}

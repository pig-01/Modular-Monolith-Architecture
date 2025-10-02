using System.Reflection;
using Autofac;

namespace Scheduler.Registers;

public class RepositoryRegisterModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder) => builder.RegisterAssemblyTypes(Assembly.Load("Scheduler.Infrastructure"))
           .Where(t => t.Name.EndsWith("Repository", StringComparison.OrdinalIgnoreCase))
           .AsImplementedInterfaces()
           .InstancePerLifetimeScope();
}

namespace Main.WebApi.Registers;

public class QueryRegisterModule : Module
{
    protected override void Load(ContainerBuilder builder) => builder.RegisterAssemblyTypes(typeof(Program).Assembly)
           .Where(t => t.Name.EndsWith("Query", StringComparison.OrdinalIgnoreCase))
           .AsImplementedInterfaces()
           .InstancePerLifetimeScope();
}

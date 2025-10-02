using System.Reflection;
using System.Text.RegularExpressions;

namespace Main.WebApi.Registers;

public class DaoRegisterModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder) => builder.RegisterAssemblyTypes(Assembly.Load("Main.Repository"))
            .Where(t => t.Name.EndsWith("Dao"))
            .Where(t => t.Namespace != null && IsValidDaoImplFormat(t.Namespace))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope()
            .AutoActivate();

    private static bool IsValidDaoImplFormat(string input)
    {
        string pattern = @"^Demo\.Demo\.Main\.Repository\.Dao\.[A-Za-z]+\.Impl$";
        return Regex.IsMatch(input, pattern);
    }
}

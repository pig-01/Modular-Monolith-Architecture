using System.Reflection;
using System.Text.RegularExpressions;
using Base.Infrastructure.Interface.Authentication;
using Base.Infrastructure.Interface.Mail;
using Base.Infrastructure.Interface.TimeZone;
using Main.Domain.AggregatesModel.UserAggregate;
using Main.WebApi.Infrastructure;

namespace Main.WebApi.Registers;

public class ServiceRegisterModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        _ = builder.RegisterAssemblyTypes(Assembly.Load("Main.Service"))
            .Where(t => t.Name.EndsWith("Service", StringComparison.OrdinalIgnoreCase))
            .Where(t => t.Namespace != null && IsValidServiceImplFormat(t.Namespace))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope()
            .AutoActivate();

        _ = builder.RegisterType<TimeZoneService>()
            .As<ITimeZoneService>()
            .InstancePerLifetimeScope()
            .AutoActivate();

        _ = builder.RegisterType<UserService>()
            .As<IUserService<Scuser>>()
            .InstancePerLifetimeScope()
            .AutoActivate();

        _ = builder.RegisterType<MailService>()
            .As<IMailService>()
            .InstancePerLifetimeScope()
            .AutoActivate();
    }

    private static bool IsValidServiceImplFormat(string input)
    {
        string pattern = @"^Demo\.Demo\.Main\.Service\.[A-Za-z0-9]+$";
        return Regex.IsMatch(input, pattern);
    }
}

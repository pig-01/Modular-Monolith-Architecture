using Autofac;
using Base.Infrastructure.Interface.Mail;
using Base.Infrastructure.Interface.TimeZone;
using Scheduler.Infrastructure;

namespace Scheduler.Registers;

public class ServiceRegisterModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        _ = builder.RegisterType<TimeZoneService>()
            .As<ITimeZoneService>()
            .InstancePerLifetimeScope()
            .AutoActivate();

        _ = builder.RegisterType<MailService>()
            .As<IMailService>()
            .InstancePerLifetimeScope()
            .AutoActivate();
    }
}

using System.Reflection;
using Autofac;
using Base.Domain.SeedWorks;
using Base.Infrastructure.Interface;
using Base.Infrastructure.Interface.Mail;
using Base.Mail.Adapter;
using Microsoft.Extensions.Configuration;

namespace Base.Mail.Module;

public class MailModule(IConfiguration configuration) : BaseModule(configuration), IModuleInitializer
{
    private readonly IConfiguration configuration = configuration;

    public override string ModuleName => nameof(MailModule);

    public string ModuleVersion => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";

    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<IMailSendAdapter>().As<MailSendAdapter>().InstancePerLifetimeScope();
    }
}

using System.Reflection;

namespace Main.WebApi.Registers;

public class ValidatorRegisterModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder) =>
        // 註冊所有 FluentValidation 的 Validator 為 Singleton
        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
               .AsClosedTypesOf(typeof(IValidator<>))
               .AsImplementedInterfaces()
               .SingleInstance();
}

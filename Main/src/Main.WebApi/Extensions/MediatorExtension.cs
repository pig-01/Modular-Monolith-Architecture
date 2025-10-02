using Main.WebApi.Application.Behaviors;

namespace Main.WebApi.Extensions;

/// <summary>
/// 擴展 Mediator
/// </summary>
public static class MediatorExtension
{
    /// <summary>
    /// 擴展 Mediator
    /// </summary>
    /// <param name="builder"></param>
    public static void AddMediatR(this WebApplicationBuilder builder) =>
        // 註冊 MediatR
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining(typeof(Program));

            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
            cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
        });
}

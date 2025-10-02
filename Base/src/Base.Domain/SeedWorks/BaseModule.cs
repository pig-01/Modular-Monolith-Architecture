using Autofac;
using Microsoft.Extensions.Configuration;

namespace Base.Domain.SeedWorks;

public abstract class BaseModule(IConfiguration configuration) : Module
{
    protected readonly IConfiguration Configuration = configuration;

    // 子類必須實現此方法，提供模組名稱
    public abstract string ModuleName { get; }

    // 子類需覆寫這個方法來註冊自己的服務
    protected abstract override void Load(ContainerBuilder builder);

    // 模組初始化方法，可在需要時覆寫
    public virtual void Initialize() { }
}

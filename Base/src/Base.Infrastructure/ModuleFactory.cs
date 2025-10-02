using Base.Infrastructure.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Base.Infrastructure;

public class ModuleFactory(IServiceProvider serviceProvider) : IModuleFactory
{
    public T GetModule<T>() where T : class => serviceProvider.GetService<T>() ??
            throw new InvalidOperationException($"Module of type {typeof(T).Name} is not registered.");

    public bool IsModuleRegistered<T>() where T : class => serviceProvider.GetService<T>() != null;
}

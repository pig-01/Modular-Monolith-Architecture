namespace Base.Infrastructure.Interface;

public interface IModuleFactory
{
    T GetModule<T>() where T : class;
    bool IsModuleRegistered<T>() where T : class;
}

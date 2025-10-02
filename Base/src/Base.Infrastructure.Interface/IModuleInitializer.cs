namespace Base.Infrastructure.Interface;

public interface IModuleInitializer
{
    /// <summary>
    /// 模組名稱
    /// </summary>
    string ModuleName { get; }

    /// <summary>
    /// 模組版本
    /// </summary>
    string ModuleVersion { get; }
}

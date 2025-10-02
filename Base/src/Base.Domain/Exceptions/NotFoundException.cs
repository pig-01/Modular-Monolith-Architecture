using Base.Domain.SeedWorks;

namespace Base.Domain.Exceptions;

/// <summary>
/// 當找不到資源時拋出的例外
/// </summary>
/// <remarks>
/// 用於表示在查找資源時未找到所需的項目。
/// 這通常用於查詢操作，當指定的資源不存在時拋出此例外。
/// </remarks>
[Serializable]
public class NotFoundException : BaseException
{
    public NotFoundException() { }
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string message, Exception inner) : base(message, inner) { }
}

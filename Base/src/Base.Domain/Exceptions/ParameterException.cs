using Base.Domain.SeedWorks;

namespace Base.Domain.Exceptions;

/// <summary>
/// 用於處理參數驗證失敗或不符合預期的情況
/// </summary>
/// <remarks>
/// 此異常類型用於在參數驗證失敗時拋出，通常用於API或方法的參數檢查。
/// 當參數不符合預期格式、範圍或其他驗證條件時，可以使用此異常來通知調用者。
/// 可以包含詳細的錯誤消息和可選的內部異常，以便於調試和錯誤處理。
/// </remarks>
[Serializable]
public class ParameterException : BaseException
{
    public ParameterException() : base() { }
    public ParameterException(string message) : base(message) { }
    public ParameterException(string message, params string[] args) : base(string.Format(message, args)) { }
    public ParameterException(string message, Exception innerException) : base(message, innerException) { }
}

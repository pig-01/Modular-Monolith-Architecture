namespace Base.Infrastructure.Interface.Progress;

public interface IBatchProgressService
{
    /// <summary>
    /// 報告批次工作進度
    /// </summary>
    /// <param name="batchId">批次識別碼</param>
    /// <param name="percentage">進度百分比</param>
    /// <param name="message">進度訊息</param>
    /// <returns></returns>
    Task ReportProgress(Guid batchId, int percentage, string? message = default);

    /// <summary>
    /// 報告批次工作完成
    /// </summary>
    /// <param name="batchId">批次識別碼</param>
    /// <param name="message">完成訊息</param>
    /// <returns></returns>
    Task ReportComplete(Guid batchId, string? message = default);

    /// <summary>
    /// 報告批次工作錯誤
    /// </summary>
    /// <param name="batchId">批次識別碼</param>
    /// <param name="message">錯誤訊息</param>
    /// <param name="err">錯誤物件</param>
    /// <returns></returns>
    Task ReportError<TError>(Guid batchId, string? message = default, TError? err = default);
}
using Main.Domain.SeedWork;

namespace Main.Domain.AggregatesModel.CustomTemplateAggregate;

public interface ICustomRequestUnitRepository : IRepository<CustomRequestUnit>
{
    /// <summary>
    /// 新增自訂要求單位
    /// </summary>
    /// <param name="unitId">要求單位識別碼</param>
    /// <param name="unitName">要求單位名稱</param>
    /// <param name="version">版本名稱</param>
    /// <param name="tenantId">站台識別碼</param>
    /// <param name="createdUser">建立人員</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">有傳入要求單位識別碼但沒有自訂要求單位</exception>
    /// <exception cref="ArgumentException">要求單位名稱為空</exception>
    Task<CustomRequestUnit> AddAsync(long? unitId, string? unitName, string version, string tenantId, string createdUser, CancellationToken cancellationToken = default);

    /// <summary>
    /// 新增單一自訂範本
    /// </summary>
    /// <param name="customTemplate">自訂範本</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns></returns>
    Task<int> AddCustomTemplateAsync(CustomPlanTemplate customTemplate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 重新命名要求單位
    /// </summary>
    /// <param name="unitId">要求單位識別碼</param>
    /// <param name="newName">要求單位新名稱</param>
    /// <param name="modifiedUser">修改人員</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns></returns>
    Task<CustomRequestUnit> RenameAsync(long unitId, string newName, string modifiedUser, CancellationToken cancellationToken = default);

    /// <summary>
    /// 重新命名要求單位版本
    /// </summary>
    /// <param name="unitId">要求單位識別碼</param>
    /// <param name="versionId">版本識別碼</param>
    /// <param name="newName">要求單位版本新名稱</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns></returns>
    Task<CustomPlanTemplateVersion> RenameVersionAsync(long unitId, long versionId, string newName, string modifiedUser, CancellationToken cancellationToken = default);

    /// <summary>
    /// 刪除要求單位
    /// </summary>
    /// <param name="unitId">要求單位識別碼</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns></returns>
    Task<int> DeleteAsync(long unitId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 刪除要求單位版本
    /// </summary>
    /// <param name="unitId">要求單位識別碼</param>
    /// <param name="versionId">版本識別碼</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns></returns>
    Task<int> DeleteVersionAsync(long unitId, long versionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 發布要求單位版本
    /// </summary>
    /// <param name="requestUnitId">要求單位識別碼</param>
    /// <param name="versionId">版本識別碼</param>
    /// <param name="deployAt">發布日期</param>
    /// <param name="modifiedUser">修改人員</param>
    /// <param name="cancellationToken">取消憑證</param>
    /// <returns></returns>
    Task<int> DeployVersionAsync(long requestUnitId, long versionId, DateTime deployAt, string modifiedUser, CancellationToken cancellationToken);
}
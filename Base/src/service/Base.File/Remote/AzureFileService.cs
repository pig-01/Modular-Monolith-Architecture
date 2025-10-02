using Base.Domain.Exceptions;
using Base.Domain.Options.FileService;
using Base.Files.Helper;
using Base.Infrastructure.Interface.Files;
using Microsoft.Extensions.Logging;

namespace Base.Files.Remote;

public class AzureFileService(IStorageAdapter adapter,
                              AzureStorageSetting settings,
                              ILogger<AzureFileService> logger,
                              IFileTypeDetector fileTypeDetector) : IFileService
{
    public async Task UploadFileAsync(Stream fileStream, string fileName, string relativePath = "", string? contentType = null)
    {
        // Azure 特有的業務邏輯
        ValidateFileName(fileName);
        await ValidateAzureConnection();
        await ValidateAzureUpload(fileStream);

        // 自動設定 Content-Type
        contentType ??= fileTypeDetector.DetectFromContent(fileStream);
        logger.LogWarning("UPLOAD {fileName} {relativePath}", fileName, relativePath);

        try
        {
            await adapter.UploadFileAsync(fileStream, fileName, relativePath ?? "", contentType);
            logger.LogInformation("Azure 檔案上傳成功: {FileName}", fileName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Azure 檔案上傳失敗: {FileName}", fileName);
            throw new FileServiceException($"Azure 檔案上傳失敗: {fileName}", ex);
        }
    }

    public async Task DeleteFileAsync(string fileName, string relativePath = "")
    {
        ValidateFileName(fileName);
        logger.LogWarning("DELETE {fileName} {relativePath}", fileName, relativePath);

        try
        {
            await adapter.DeleteFileAsync(fileName, relativePath ?? "");
            logger.LogInformation("Azure 檔案刪除成功: {FileName}", fileName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Azure 檔案刪除失敗: {FileName}", fileName);
            throw new FileServiceException($"Azure 檔案刪除失敗: {fileName}", ex);
        }
    }

    public async Task<Stream> GetFileAsync(string fileName, string filePath, string relativePath = "")
    {
        ValidateFileName(fileName);
        await ValidateAzureConnection();
        logger.LogWarning("GET {fileName} {relativePath}", fileName, relativePath);

        try
        {
            return await adapter.GetFileAsync(fileName, relativePath ?? "");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Azure 檔案取得失敗: {FileName}, 路徑: {RelativePath}", fileName, relativePath);
            throw new FileServiceException($"Azure 檔案取得失敗: {fileName}", ex);
        }
    }

    public async Task<bool> ExistsFileAsync(string fileName, string relativePath = "")
    {
        ValidateFileName(fileName);
        return await adapter.ExistsFileAsync(fileName, relativePath ?? "");
    }

    public async Task<IEnumerable<string>> ListFilesAsync(string relativePath = "") => await adapter.ListFilesAsync(relativePath ?? "");

    // Azure 特有的驗證方法
    private static void ValidateFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("檔案名稱不能為空", nameof(fileName));
    }

    private async Task ValidateAzureConnection()
    {
        if (!await adapter.ValidateConnectionAsync())
            throw new InvalidOperationException("Azure Storage 連線失敗，請檢查連線字串");
    }

    private async Task ValidateAzureUpload(Stream fileStream)
    {
        ArgumentNullException.ThrowIfNull(fileStream);

        // Azure Blob 大小限制檢查 (例如: 5GB for block blob)
        if (fileStream.Length > settings.MaxFileSizeBytes)
            throw new ArgumentException("檔案大小超過 Azure Blob 限制 (100MB)", nameof(fileStream));
    }

}

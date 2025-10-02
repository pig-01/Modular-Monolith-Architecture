using Base.Domain.Exceptions;
using Base.Domain.Options.FileService;
using Base.Infrastructure.Interface.Files;
using Base.Infrastructure.Toolkits.Resources;
using Microsoft.Extensions.Logging;

namespace Base.Files.Local;

public class LocalFileService(IStorageAdapter adapter, LocalStorageSetting settings, ILogger<LocalFileService> logger) : IFileService
{

    public async Task<Stream> GetFileAsync(string fileName, string filePath, string relativePath = "")
    {
        // 業務邏輯驗證
        ValidateFileName(fileName);
        ValidateLocalPath(relativePath);
        CheckDiskSpace();

        try
        {
            logger.LogInformation("正在從本地取得檔案: {FileName}, 路徑: {RelativePath}", fileName, relativePath);
            return await adapter.GetFileAsync(fileName, relativePath ?? "");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "本地檔案取得失敗: {FileName}, 路徑: {RelativePath}", fileName, relativePath);
            throw new FileServiceException($"本地檔案取得失敗: {fileName}", ex);
        }
    }


    public async Task UploadFileAsync(Stream fileStream, string fileName, string relativePath = "", string? contentType = null)
    {
        // 業務邏輯驗證
        ValidateFileName(fileName);
        ValidateLocalPath(relativePath);
        CheckDiskSpace();

        try
        {
            logger.LogInformation("正在上傳檔案到本地: {FileName}, 路徑: {RelativePath}", fileName, relativePath);
            await adapter.UploadFileAsync(fileStream, fileName, relativePath ?? "", contentType);
            logger.LogInformation("本地檔案上傳成功: {FileName}", fileName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "本地檔案上傳失敗: {FileName}, 路徑: {RelativePath}", fileName, relativePath);
            throw new FileServiceException($"本地檔案上傳失敗: {fileName}", ex);
        }
    }

    public async Task DeleteFileAsync(string fileName, string relativePath = "")
    {
        // 業務邏輯驗證
        ValidateFileName(fileName);
        ValidateLocalPath(relativePath);

        try
        {
            await adapter.DeleteFileAsync(fileName, relativePath ?? "");
            logger.LogInformation("本地檔案刪除成功: {FileName}", fileName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "本地檔案刪除失敗: {FileName}", fileName);
            throw new FileServiceException($"本地檔案刪除失敗: {fileName}", ex);
        }
    }

    public async Task<bool> ExistsFileAsync(string fileName, string relativePath = "")
    {
        ValidateFileName(fileName);
        ValidateLocalPath(relativePath);
        return await adapter.ExistsFileAsync(fileName, relativePath ?? "");
    }

    public async Task<IEnumerable<string>> ListFilesAsync(string relativePath = "")
    {
        ValidateLocalPath(relativePath);
        return await adapter.ListFilesAsync(relativePath ?? "");
    }

    // Local 特有的驗證方法
    private static void ValidateFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("檔案名稱不能為空", nameof(fileName));

        // 檢查不安全字元
        char[] invalidChars = Path.GetInvalidFileNameChars();
        if (fileName.IndexOfAny(invalidChars) >= 0)
            throw new ArgumentException("檔案名稱包含不合法字元", nameof(fileName));
    }

    private static void ValidateLocalPath(string? relativePath)
    {
        if (string.IsNullOrEmpty(relativePath)) return;

        // 防止路徑穿越攻擊 (Path Traversal)
        if (relativePath.Contains("..") || relativePath.Contains("~") || relativePath.StartsWith("/") || relativePath.Contains(":"))
            throw new ArgumentException("不安全的路徑", nameof(relativePath));
    }

    private static void ValidateLocalUpload(Stream fileStream, string fileName)
    {
        if (fileStream == null) throw new ArgumentNullException(nameof(fileStream));

        if (fileStream.Length > 100 * 1024 * 1024)
            throw new ArgumentException("檔案大小超過限制 (100MB)", nameof(fileStream));
    }

    private void CheckDiskSpace()
    {
        try
        {
            DriveInfo drive = new DriveInfo(Path.GetPathRoot(settings.LocalFilePath) ?? "C:");
            if (drive.AvailableFreeSpace < 100 * 1024 * 1024) // 100MB
                throw new InvalidOperationException("磁碟空間不足，需要至少 100MB 可用空間");
        }
        catch (Exception ex) when (!(ex is InvalidOperationException))
        {
            logger.LogWarning(ex, "無法檢查磁碟空間");
        }
    }

}

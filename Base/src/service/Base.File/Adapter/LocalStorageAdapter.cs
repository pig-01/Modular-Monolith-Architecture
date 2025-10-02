using Base.Domain.Options.FileService;
using Base.Infrastructure.Interface.Files;
using Microsoft.Extensions.Logging;

namespace Base.Files.Adapter;

public class LocalStorageAdapter(LocalStorageSetting settings, ILogger<LocalStorageAdapter> logger) : IStorageAdapter
{
    private readonly string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, settings.LocalFilePath);

    public async Task<Stream> GetFileAsync(string fileName, string relativePath)
    {
        string fullPath = GetFullPath(fileName, relativePath);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"檔案不存在: {fullPath}");

        return File.OpenRead(fullPath);
    }

    public async Task UploadFileAsync(Stream stream, string fileName, string relativePath, string? contentType = default)
    {
        string fullPath = GetFullPath(fileName, relativePath);
        string? directory = Path.GetDirectoryName(fullPath);

        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        using FileStream fileStream = File.Create(fullPath);
        await stream.CopyToAsync(fileStream);

        logger.LogInformation("已上傳檔案到本地: {FullPath}", fullPath);
    }

    public async Task DeleteFileAsync(string fileName, string relativePath)
    {
        string fullPath = GetFullPath(fileName, relativePath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            logger.LogInformation("已刪除本地檔案: {FullPath}", fullPath);
        }
    }

    public async Task<bool> ExistsFileAsync(string fileName, string relativePath)
    {
        string fullPath = GetFullPath(fileName, relativePath);
        return File.Exists(fullPath);
    }

    public async Task<IEnumerable<string>> ListFilesAsync(string relativePath)
    {
        string targetPath = string.IsNullOrEmpty(relativePath)
            ? basePath
            : Path.Combine(basePath, relativePath);

        if (!Directory.Exists(targetPath))
            return Enumerable.Empty<string>();

        return Directory.GetFiles(targetPath)
            .Select(Path.GetFileName)
            .Where(name => !string.IsNullOrEmpty(name))!;
    }

    public async Task<bool> ValidateConnectionAsync()
    {
        try
        {
            // 檢查基礎路徑是否可存取
            Directory.CreateDirectory(basePath);
            return Directory.Exists(basePath);
        }
        catch
        {
            return false;
        }
    }

    private string GetFullPath(string fileName, string relativePath)
    {
        string path = string.IsNullOrEmpty(relativePath)
            ? Path.Combine(basePath, fileName)
            : Path.Combine(basePath, relativePath, fileName);

        return Path.GetFullPath(path);
    }
}
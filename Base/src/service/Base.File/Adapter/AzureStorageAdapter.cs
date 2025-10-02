using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Base.Domain.Options.FileService;
using Base.Infrastructure.Interface.Files;
using Microsoft.Extensions.Logging;

namespace Base.Files.Adapter;

public class AzureStorageAdapter(AzureStorageSetting settings, ILogger<AzureStorageAdapter> logger) : IStorageAdapter
{
    private readonly BlobServiceClient blobServiceClient = new(settings.ConnectionString);
    private readonly string containerName = settings.ContainerName ?? "files";

    /// <summary>
    /// 從 Azure Blob Storage 取得檔案流
    /// </summary>
    /// <param name="fileName">檔案名稱（通常是 GUID + 副檔名）</param>
    /// <param name="relativePath">相對路徑（Azure 模式下此參數被忽略）</param>
    /// <returns>檔案流</returns>
    public async Task<Stream> GetFileAsync(string fileName, string relativePath)
    {
        string blobName = GetBlobName(fileName);
        BlobContainerClient containerClient = await GetContainerClientAsync();
        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        if (!await blobClient.ExistsAsync())
            throw new FileNotFoundException($"Azure Blob 不存在: {blobName}");

        Azure.Response<BlobDownloadStreamingResult> response = await blobClient.DownloadStreamingAsync();
        return response.Value.Content;
    }

    /// <summary>
    /// 上傳檔案到 Azure Blob Storage
    /// </summary>
    /// <param name="stream">檔案流</param>
    /// <param name="fileName">檔案名稱（通常是 GUID + 副檔名）</param>
    /// <param name="relativePath">相對路徑（Azure 模式下此參數被忽略）</param>
    /// <param name="contentType">檔案 MIME 類型，用於設定 Azure Blob 的 ContentType 屬性</param>
    /// <returns>上傳任務</returns>
    public async Task UploadFileAsync(Stream stream, string fileName, string relativePath, string? contentType = default)
    {
        string blobName = GetBlobName(fileName);
        BlobContainerClient containerClient = await GetContainerClientAsync();
        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        BlobUploadOptions options = new();
        if (!string.IsNullOrEmpty(contentType))
        {
            options.HttpHeaders = new BlobHttpHeaders { ContentType = contentType };
        }

        await blobClient.UploadAsync(stream, options);
        logger.LogInformation("已上傳檔案到 Azure Blob: {BlobName}", blobName);
    }

    /// <summary>
    /// 從 Azure Blob Storage 刪除檔案
    /// </summary>
    /// <param name="fileName">檔案名稱（通常是 GUID + 副檔名）</param>
    /// <param name="relativePath">相對路徑（Azure 模式下此參數被忽略）</param>
    /// <returns>刪除任務</returns>
    public async Task DeleteFileAsync(string fileName, string relativePath)
    {
        string blobName = GetBlobName(fileName);
        BlobContainerClient containerClient = await GetContainerClientAsync();
        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        await blobClient.DeleteIfExistsAsync();
        logger.LogInformation("已刪除 Azure Blob: {BlobName}", blobName);
    }

    /// <summary>
    /// 檢查檔案是否存在於 Azure Blob Storage
    /// </summary>
    /// <param name="fileName">檔案名稱（通常是 GUID + 副檔名）</param>
    /// <param name="relativePath">相對路徑（Azure 模式下此參數被忽略）</param>
    /// <returns>檔案是否存在</returns>
    public async Task<bool> ExistsFileAsync(string fileName, string relativePath)
    {
        string blobName = GetBlobName(fileName);
        BlobContainerClient containerClient = await GetContainerClientAsync();
        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        Azure.Response<bool> response = await blobClient.ExistsAsync();
        return response.Value;
    }

    /// <summary>
    /// 列出 Azure Container 中的所有檔案
    /// </summary>
    /// <param name="relativePath">相對路徑（Azure 模式下此參數被忽略，會列出所有檔案）</param>
    /// <returns>所有檔案名稱的集合</returns>
    public async Task<IEnumerable<string>> ListFilesAsync(string relativePath)
    {
        BlobContainerClient containerClient = await GetContainerClientAsync();
        Azure.AsyncPageable<BlobItem> blobs = containerClient.GetBlobsAsync();
        List<string> fileNames = [];

        await foreach (BlobItem blob in blobs)
        {
            // Azure 模式：所有檔案都是 GUID 名稱，沒有路徑結構
            fileNames.Add(blob.Name);
        }

        // 記錄日誌說明行為
        logger.LogInformation("Azure 模式列出 {Count} 個檔案，忽略路徑參數: {RelativePath}",
            fileNames.Count, relativePath);

        return fileNames;
    }

    /// <summary>
    /// 驗證與 Azure Storage 的連線狀態
    /// </summary>
    /// <returns>連線是否正常</returns>
    public async Task<bool> ValidateConnectionAsync()
    {
        try
        {
            await blobServiceClient.GetPropertiesAsync();
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Azure Storage 連線驗證失敗");
            return false;
        }
    }

    /// <summary>
    /// 取得 Azure Blob Container 客戶端，如果 Container 不存在會自動建立
    /// </summary>
    /// <returns>Azure Blob Container 客戶端</returns>
    private async Task<BlobContainerClient> GetContainerClientAsync()
    {
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);
        return containerClient;
    }

    /// <summary>
    /// 取得 Azure Blob 名稱（Azure 模式下直接使用檔案名稱，不包含路徑）
    /// </summary>
    /// <param name="fileName">檔案名稱</param>
    /// <returns>Blob 名稱</returns>
    private static string GetBlobName(string fileName) => fileName;
}
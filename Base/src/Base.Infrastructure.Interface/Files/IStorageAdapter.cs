namespace Base.Infrastructure.Interface.Files;

public interface IStorageAdapter
{
    Task<Stream> GetFileAsync(string fileName, string relativePath);
    Task UploadFileAsync(Stream stream, string fileName, string relativePath, string? contentType = default);
    Task DeleteFileAsync(string fileName, string relativePath);
    Task<bool> ExistsFileAsync(string fileName, string relativePath);
    Task<IEnumerable<string>> ListFilesAsync(string relativePath);
    Task<bool> ValidateConnectionAsync();
}
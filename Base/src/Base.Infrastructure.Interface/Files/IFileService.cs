namespace Base.Infrastructure.Interface.Files;

public interface IFileService
{
    Task<Stream> GetFileAsync(string fileName, string filePath, string relativePath = "");

    Task UploadFileAsync(Stream fileStream, string fileName, string relativePath = "", string? contentType = default);

    Task DeleteFileAsync(string fileName, string relativePath = "");

    Task<bool> ExistsFileAsync(string fileName, string relativePath = "");

    Task<IEnumerable<string>> ListFilesAsync(string relativePath = "");

}

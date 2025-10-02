using Microsoft.AspNetCore.Http;
using MimeDetective;

namespace Base.Files.Helper;

public class MimeDetectiveFileTypeDetector : IFileTypeDetector
{
    private readonly IContentInspector inspector;

    public MimeDetectiveFileTypeDetector() => inspector = new ContentInspectorBuilder()
    {
        Definitions = MimeDetective.Definitions.DefaultDefinitions.All()
    }.Build();

    public string DetectFromContent(Stream stream)
    {
        long originalPosition = stream.Position;
        try
        {
            stream.Position = 0;
            var results = inspector.Inspect(stream);
            return results.FirstOrDefault()?.Definition.File.MimeType ?? "application/octet-stream";
        }
        finally
        {
            stream.Position = originalPosition;
        }
    }
    public string DetectFromContent(IFormFile file)
    {
        using Stream stream = file.OpenReadStream();
        return DetectFromContent(stream);
    }

    public string DetectFromFileName(string fileName)
    {
        // 備用方法：根據副檔名判斷
        string extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".pdf" => "application/pdf",
            ".txt" => "text/plain",
            ".json" => "application/json",
            ".xml" => "application/xml",
            ".zip" => "application/zip",
            _ => "application/octet-stream"
        };
    }
}

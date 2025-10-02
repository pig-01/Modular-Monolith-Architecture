using System;
using Microsoft.AspNetCore.Http;

namespace Base.Files.Helper;

public interface IFileTypeDetector
{
    string DetectFromContent(Stream stream);
    string DetectFromContent(IFormFile file);
    string DetectFromFileName(string fileName);
}

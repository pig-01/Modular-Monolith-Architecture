
using Base.Domain.Enums;

namespace Base.Files.Helper;

/// <summary>
/// MIME 類型分類器與擴充方法
/// </summary>
public static class MimeTypeClassifier
{
    private static readonly Dictionary<FileCategoryEnum, HashSet<string>> ExactMappings = new()
    {
        [FileCategoryEnum.Image] =
        [
            "image/jpeg", "image/jpg", "image/png", "image/gif",
            "image/bmp", "image/webp", "image/svg+xml", "image/tiff",
            "image/x-icon", "image/vnd.microsoft.icon"
        ],

        [FileCategoryEnum.Document] =
        [
            "application/pdf",
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/vnd.ms-excel",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "application/vnd.ms-powerpoint",
            "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            "application/vnd.oasis.opendocument.text",
            "application/vnd.oasis.opendocument.spreadsheet",
            "application/vnd.oasis.opendocument.presentation",
            "text/plain",
            "text/rtf",
            "text/csv"
        ],

        [FileCategoryEnum.Video] =
        [
            "video/mp4", "video/mpeg", "video/quicktime",
            "video/x-msvideo", "video/x-ms-wmv", "video/webm",
            "video/x-flv", "video/ogg", "video/3gpp",
            "video/x-matroska"
        ],

        [FileCategoryEnum.Audio] =
        [
            "audio/mpeg", "audio/mp3", "audio/wav",
            "audio/ogg", "audio/webm", "audio/flac",
            "audio/x-m4a", "audio/mp4", "audio/aac",
            "audio/x-ms-wma"
        ],

        [FileCategoryEnum.Archive] =
        [
            "application/zip", "application/x-rar-compressed",
            "application/x-7z-compressed", "application/x-tar",
            "application/gzip", "application/x-bzip2",
            "application/x-xz", "application/x-compressed"
        ],

        [FileCategoryEnum.Code] =
        [
            "text/html", "text/css", "text/javascript",
            "application/javascript", "application/json",
            "application/xml", "text/xml", "application/x-yaml",
            "text/x-python", "text/x-java-source",
            "text/x-csharp", "application/typescript"
        ]
    };

    /// <summary>
    /// 取得檔案分類
    /// </summary>
    /// <param name="contentType">MIME 類型</param>
    /// <returns>檔案分類</returns>
    public static FileCategoryEnum GetFileCategory(string contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            return FileCategoryEnum.Other;

        var normalizedType = contentType.ToLowerInvariant().Split(';')[0].Trim();

        // 1. 前綴快速判斷（處理 80% 的情況）
        var category = normalizedType switch
        {
            string t when t.StartsWith("image/") => FileCategoryEnum.Image,
            string t when t.StartsWith("video/") => FileCategoryEnum.Video,
            string t when t.StartsWith("audio/") => FileCategoryEnum.Audio,
            string t when t.StartsWith("text/html") || t.StartsWith("text/css") => FileCategoryEnum.Code,
            string t when t.StartsWith("text/") => FileCategoryEnum.Document,
            _ => FileCategoryEnum.Other
        };

        if (category != FileCategoryEnum.Other)
            return category;

        // 2. 精確匹配（處理特殊 MIME 類型）
        foreach (var mapping in ExactMappings)
        {
            if (mapping.Value.Contains(normalizedType))
                return mapping.Key;
        }

        return FileCategoryEnum.Other;
    }

    /// <summary>
    /// 取得分類的友善名稱
    /// </summary>
    public static string GetCategoryDisplayName(FileCategoryEnum category)
    {
        return category switch
        {
            FileCategoryEnum.Image => "圖片",
            FileCategoryEnum.Document => "文件",
            FileCategoryEnum.Video => "影片",
            FileCategoryEnum.Audio => "音訊",
            FileCategoryEnum.Archive => "壓縮檔",
            FileCategoryEnum.Code => "程式碼",
            _ => "其他"
        };
    }

    #region 擴充方法

    /// <summary>
    /// 取得 MIME 類型的分類
    /// </summary>
    public static FileCategoryEnum GetCategory(this string mimeType)
        => GetFileCategory(mimeType);

    /// <summary>
    /// 判斷是否為圖片
    /// </summary>
    public static bool IsImage(this string mimeType)
        => GetFileCategory(mimeType) == FileCategoryEnum.Image;

    /// <summary>
    /// 判斷是否為文件
    /// </summary>
    public static bool IsDocument(this string mimeType)
        => GetFileCategory(mimeType) == FileCategoryEnum.Document;

    /// <summary>
    /// 判斷是否為影片
    /// </summary>
    public static bool IsVideo(this string mimeType)
        => GetFileCategory(mimeType) == FileCategoryEnum.Video;

    /// <summary>
    /// 判斷是否為音訊
    /// </summary>
    public static bool IsAudio(this string mimeType)
        => GetFileCategory(mimeType) == FileCategoryEnum.Audio;

    /// <summary>
    /// 判斷是否為壓縮檔
    /// </summary>
    public static bool IsArchive(this string mimeType)
        => GetFileCategory(mimeType) == FileCategoryEnum.Archive;

    /// <summary>
    /// 判斷是否為程式碼
    /// </summary>
    public static bool IsCode(this string mimeType)
        => GetFileCategory(mimeType) == FileCategoryEnum.Code;

    #endregion
}
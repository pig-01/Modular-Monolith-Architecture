namespace Base.Domain.Options.FileService;

public struct LocalStorageSetting()
{
    public string LocalFilePath { get; set; } = string.Empty;

    public long MaxFileSizeBytes { get; set; }
}

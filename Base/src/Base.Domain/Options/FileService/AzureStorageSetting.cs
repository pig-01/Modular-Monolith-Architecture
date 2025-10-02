namespace Base.Domain.Options.FileService;

public struct AzureStorageSetting()
{
    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
    public long MaxFileSizeBytes { get; set; }
}

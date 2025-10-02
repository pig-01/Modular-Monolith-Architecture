using static Base.Domain.Enums.StorageModeEnum;

namespace Base.Domain.Options.FileService;

public class FileServiceOption()
{
    public const string Position = "FileService";

    public StorageMode Mode { get; set; } = StorageMode.Local;
    public LocalStorageSetting LocalStorageSetting { get; set; } = new LocalStorageSetting();
    public AzureStorageSetting AzureStorageSetting { get; set; } = new AzureStorageSetting();

}

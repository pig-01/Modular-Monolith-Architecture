namespace Base.Domain.Options.Cryptography;

public struct SymmetricAlgorithmSetting()
{
    public int KeySize { get; set; } = 256;
    public int MacSize { get; set; } = 128;
    public int NonceSize { get; set; } = 128;
    public string Key { get; set; } = "**********************************************";
}

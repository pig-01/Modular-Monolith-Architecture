namespace Base.Domain.Options.Cryptography;

public class CryptographyOption()
{
    public const string Position = "Cryptography";

    public HashAlgorithmSetting HashAlgorithmSetting { get; set; } = new HashAlgorithmSetting();
    public SymmetricAlgorithmSetting SymmetricAlgorithmSetting { get; set; } = new SymmetricAlgorithmSetting();
    public AsymmetricAlgorithmSetting AsymmetricAlgorithmSetting { get; set; } = new AsymmetricAlgorithmSetting();
}

namespace Base.Infrastructure.Interface.Security.Cryptography;

public interface IAsymmetricAlgorithmService
{
    /// <summary>
    /// 匯出公開金鑰 (PEM 格式)
    /// </summary>
    string ExportPublicKey();


    /// <summary>
    /// 匯出私密金鑰 (PEM 格式)
    /// </summary>
    string ExportPrivateKey();


    /// <summary>
    /// 匯入公開金鑰 (PEM 格式)
    /// </summary>
    void ImportPublicKey(string pemKey);


    /// <summary>
    /// 匯入私密金鑰 (PEM 格式)
    /// </summary>
    void ImportPrivateKey(string pemKey);


    /// <summary>
    /// 加密文字內容 (輸出 Base64 字串)
    /// </summary>
    string Encrypt(string plainText);


    /// <summary>
    /// 解密 Base64 編碼的加密字串
    /// </summary>
    string Decrypt(string encryptedText);
}

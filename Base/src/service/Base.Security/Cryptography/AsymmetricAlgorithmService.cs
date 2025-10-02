using System.Security.Cryptography;
using System.Text;
using Base.Domain.Options.Cryptography;
using Base.Domain.SeedWorks;
using Base.Infrastructure.Interface.Security.Cryptography;

namespace Base.Security.Cryptography;

/// <summary>
/// 非對稱式加密服務
/// </summary>
public class AsymmetricAlgorithmService : BaseService, IAsymmetricAlgorithmService
{
    private readonly RSA rsa;
    public AsymmetricAlgorithmService(AsymmetricAlgorithmSetting setting)
    {
        rsa = RSA.Create();
        rsa.KeySize = setting.KeySize;
    }

    /// <summary>
    /// 匯出公開金鑰 (PEM 格式)
    /// </summary>
    public string ExportPublicKey()
    {
        byte[] publicKey = rsa.ExportSubjectPublicKeyInfo();
        return ConvertToPemFormat(publicKey, "PUBLIC KEY");
    }

    /// <summary>
    /// 匯出私密金鑰 (PEM 格式)
    /// </summary>
    public string ExportPrivateKey()
    {
        byte[] privateKey = rsa.ExportPkcs8PrivateKey();
        return ConvertToPemFormat(privateKey, "PRIVATE KEY");
    }

    /// <summary>
    /// 匯入公開金鑰 (PEM 格式)
    /// </summary>
    public void ImportPublicKey(string pemKey)
    {
        byte[] publicKey = ConvertFromPemFormat(pemKey);
        rsa.ImportSubjectPublicKeyInfo(publicKey, out _);
    }

    /// <summary>
    /// 匯入私密金鑰 (PEM 格式)
    /// </summary>
    public void ImportPrivateKey(string pemKey)
    {
        byte[] privateKey = ConvertFromPemFormat(pemKey);
        rsa.ImportPkcs8PrivateKey(privateKey, out _);
    }

    /// <summary>
    /// 加密文字內容 (輸出 Base64 字串)
    /// </summary>
    public string Encrypt(string plainText)
    {
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] encryptedBytes = rsa.Encrypt(plainBytes, RSAEncryptionPadding.OaepSHA256);
        return Convert.ToBase64String(encryptedBytes);
    }

    /// <summary>
    /// 解密 Base64 編碼的加密字串
    /// </summary>
    public string Decrypt(string encryptedText)
    {
        byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
        byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
        return Encoding.UTF8.GetString(decryptedBytes);
    }

    /// <summary>
    /// 轉換位元組陣列為 PEM 格式字串
    /// </summary>
    private static string ConvertToPemFormat(byte[] keyBytes, string keyType)
    {
        string base64Key = Convert.ToBase64String(keyBytes);
        StringBuilder sb = new();
        sb.AppendLine($"-----BEGIN {keyType}-----");
        for (int i = 0; i < base64Key.Length; i += 64)
        {
            sb.AppendLine(base64Key.Substring(i, Math.Min(64, base64Key.Length - i)));
        }
        sb.AppendLine($"-----END {keyType}-----");
        return sb.ToString();
    }

    /// <summary>
    /// 從 PEM 格式字串轉換為位元組陣列
    /// </summary>
    private static byte[] ConvertFromPemFormat(string pemKey)
    {
        string base64Key = pemKey.Replace("-----BEGIN PUBLIC KEY-----", "")
                              .Replace("-----END PUBLIC KEY-----", "")
                              .Replace("-----BEGIN PRIVATE KEY-----", "")
                              .Replace("-----END PRIVATE KEY-----", "")
                              .Replace("\n", "")
                              .Replace("\r", "");
        return Convert.FromBase64String(base64Key);
    }

    public override void Dispose(bool disposing)
    {
        rsa.Dispose();
        base.Dispose(disposing);
    }
}

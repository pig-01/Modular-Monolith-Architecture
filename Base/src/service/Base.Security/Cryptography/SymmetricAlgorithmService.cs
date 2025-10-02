using System.Security.Cryptography;
using System.Text;
using Base.Domain.Options.Cryptography;
using Base.Domain.SeedWorks;
using Base.Infrastructure.Interface.Security.Cryptography;
using Base.Infrastructure.Toolkits.Extensions;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Base.Security.Cryptography;

/// <summary>
/// 對稱式加密服務
/// </summary>
public class SymmetricAlgorithmService(SymmetricAlgorithmSetting symmetricAlgorithmSetting) : BaseService, ISymmetricAlgorithmService
{
    private const int DEFAULT_KEY_BIT_SIZE = 256;
    private const int DEFAULT_MAC_BIT_SIZE = 128;
    private const int DEFAULT_NONCE_BIT_SIZE = 128;

    private int _keySize = symmetricAlgorithmSetting.KeySize;
    private int _macSize = symmetricAlgorithmSetting.MacSize;
    private int _nonceSize = symmetricAlgorithmSetting.NonceSize;


    /// <summary>
    /// 對UTF-8編碼的密文進行簡單解密並驗證(AES-GCM)
    /// </summary>
    /// <param name="encryptedMessage">UTF-8編碼的密文</param>
    /// <param name="key">Base64編碼的256位金鑰</param>
    /// <param name="nonSecretPayload">可選的非祕密有效載荷</param>
    /// <returns>UTF-8編碼的明文</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public string DecryptWithKey(string encryptedMessage, string? key = null, string? nonSecretPayload = null)
    {
        if (string.IsNullOrWhiteSpace(encryptedMessage))
        {
            throw new ArgumentNullException(nameof(encryptedMessage));
        }

        byte[] decodedKey = Encoding.UTF8.GetBytes(key ?? symmetricAlgorithmSetting.Key);

        byte[] cipherText = Convert.FromBase64String(encryptedMessage);

        byte[] plainText = DecryptWithKey(cipherText, decodedKey, Encoding.UTF8.GetBytes(nonSecretPayload ?? ""));

        return Encoding.UTF8.GetString(plainText);
    }

    /// <summary>
    /// 對UTF-8編碼的明文進行簡單加密(AES-GCM)
    /// </summary>
    /// <param name="messageToEncrypt">UTF-8編碼的明文</param>
    /// <param name="key">Base64編碼的256位金鑰</param>
    /// <param name="nonSecretPayload">可選的非祕密有效載荷</param>
    /// <returns>UTF-8編碼的密文</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public string EncryptWithKey(string messageToEncrypt, string? key = null, string? nonSecretPayload = null)
    {
        if (string.IsNullOrWhiteSpace(messageToEncrypt))
        {
            throw new ArgumentNullException(nameof(messageToEncrypt));
        }

        byte[] decodedKey = Encoding.UTF8.GetBytes(key ?? symmetricAlgorithmSetting.Key);

        byte[] plainText = Encoding.UTF8.GetBytes(messageToEncrypt);

        byte[] cipherText = EncryptWithKey(plainText, decodedKey, Encoding.UTF8.GetBytes(nonSecretPayload ?? ""));

        return Convert.ToBase64String(cipherText);
    }


    /// <summary>
    /// AES-GCM 解密並驗證
    /// </summary>
    /// <param name="encryptedMessage">密文位元組</param>
    /// <param name="key">金鑰位元組</param>
    /// <param name="nonSecretPayload">非有效載荷位元組</param>
    /// <returns>明文位元組</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="CryptographicException"></exception>
    private byte[] DecryptWithKey(byte[] encryptedMessage, byte[] key, byte[] nonSecretPayload)
    {
        CheckKey(ref key);

        if (encryptedMessage == null || encryptedMessage.Length == 0)
        {
            throw new ArgumentNullException(nameof(encryptedMessage));
        }

        using MemoryStream cipherStream = new(encryptedMessage);
        using BinaryReader cipherReader = new(cipherStream);

        // 讀取 Non-secret Payload
        int nonSecretPayloadLength = nonSecretPayload.Length;
        byte[] nonSecretPayloadNew = cipherReader.ReadBytes(nonSecretPayloadLength);

        if (!nonSecretPayloadNew.Equals<byte>(nonSecretPayload))
        {
            throw new CryptographicException("Non-secret payload does not match.");
        }

        // 讀取 Nonce
        byte[] nonce = cipherReader.ReadBytes(_nonceSize / 8);

        GcmBlockCipher cipher = new(new AesEngine());
        AeadParameters parameters = new(new KeyParameter(key), _macSize, nonce, nonSecretPayload);
        cipher.Init(false, parameters);

        // 讀取密文
        byte[] cipherText = cipherReader.ReadBytes(encryptedMessage.Length - nonSecretPayloadLength - nonce.Length);
        byte[] plainText = new byte[cipher.GetOutputSize(cipherText.Length)];

        int len = cipher.ProcessBytes(cipherText, 0, cipherText.Length, plainText, 0);
        cipher.DoFinal(plainText, len);

        return plainText;
    }

    /// <summary>
    /// AES-GCM 加密
    /// </summary>
    /// <param name="messageToEncrypt">明文位元組</param>
    /// <param name="key">金鑰位元組</param>
    /// <param name="nonSecretPayload">非有效載荷位元組</param>
    /// <returns>密文位元組</returns>
    /// <exception cref="ArgumentNullException"></exception>
    private byte[] EncryptWithKey(byte[] messageToEncrypt, byte[] key, byte[] nonSecretPayload)
    {
        CheckKey(ref key);

        if (messageToEncrypt == null || messageToEncrypt.Length == 0)
        {
            throw new ArgumentNullException(nameof(messageToEncrypt));
        }

        using MemoryStream cipherStream = new();
        using BinaryWriter cipherWriter = new(cipherStream);

        // 寫入 Non-secret Payload
        cipherWriter.Write(nonSecretPayload);

        // 產生 Nonce
        byte[] nonce = new byte[_nonceSize / 8];
        new SecureRandom().NextBytes(nonce);
        cipherWriter.Write(nonce);

        GcmBlockCipher cipher = new(new AesEngine());
        AeadParameters parameters = new(new KeyParameter(key), _macSize, nonce, nonSecretPayload);
        cipher.Init(true, parameters);

        // 寫入密文
        byte[] cipherText = new byte[cipher.GetOutputSize(messageToEncrypt.Length)];
        int len = cipher.ProcessBytes(messageToEncrypt, 0, messageToEncrypt.Length, cipherText, 0);
        cipher.DoFinal(cipherText, len);

        cipherWriter.Write(cipherText);

        return cipherStream.ToArray();
    }

    /// <summary>
    /// 檢查金鑰是否符合規定，不符合則修正
    /// </summary>
    /// <param name="key">金鑰</param>
    private void CheckKey(ref byte[] key)
    {
        if (key == null || key.Length != _keySize / 8)
        {
            key = CopyOfRange(key!, _keySize / 8);
        }
    }

    /// <summary>
    /// 將原始陣列複製到指定長度的新陣列中，補足到目標長度
    /// </summary>
    /// <param name="source">原始陣列</param>
    /// <param name="len">目標長度</param>
    /// <returns>新陣列</returns>
    private static byte[] CopyOfRange(byte[] source, int len)
    {
        byte[] range = new byte[len];

        if (source.Length >= len)
        {
            Array.Copy(source, range, len);
        }
        else
        {
            Array.Copy(source, range, source.Length);
        }

        return range;
    }

    public override void Dispose(bool disposing)
    {
        _keySize = DEFAULT_KEY_BIT_SIZE;
        _macSize = DEFAULT_MAC_BIT_SIZE;
        _nonceSize = DEFAULT_NONCE_BIT_SIZE;
        base.Dispose(disposing);
    }
}

namespace Base.Infrastructure.Interface.Security.Cryptography;

public interface ISymmetricAlgorithmService
{

    /// <summary>
    /// 對UTF-8編碼的密文進行簡單解密並驗證(AES-GCM)
    /// </summary>
    /// <param name="encryptedMessage">UTF-8編碼的密文</param>
    /// <param name="key">Base64編碼的256位金鑰</param>
    /// <param name="nonSecretPayload">可選的非祕密有效載荷</param>
    /// <returns>UTF-8編碼的明文</returns>
    /// <exception cref="ArgumentNullException"></exception>
    string DecryptWithKey(string encryptedMessage, string? key = null, string? nonSecretPayload = null);

    /// <summary>
    /// 對UTF-8編碼的明文進行簡單加密(AES-GCM)
    /// </summary>
    /// <param name="messageToEncrypt">UTF-8編碼的明文</param>
    /// <param name="key">Base64編碼的256位金鑰</param>
    /// <param name="nonSecretPayload">可選的非祕密有效載荷</param>
    /// <returns>UTF-8編碼的密文</returns>
    /// <exception cref="ArgumentNullException"></exception>
    string EncryptWithKey(string messageToEncrypt, string? key = null, string? nonSecretPayload = null);
}

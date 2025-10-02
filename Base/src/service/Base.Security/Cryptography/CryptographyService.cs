using System.Security.Cryptography;
using System.Text;
using Base.Domain.SeedWorks;
using Base.Infrastructure.Interface.Security.Cryptography;
using Base.Infrastructure.Toolkits.Extensions;

namespace Base.Security.Cryptography;

public class CryptographyService : BaseService, ICryptographyService
{
    private const string DEFAULT_SALT = "@@@@@@@@";

    private static T CryptoProviderFactory<T>() => Activator.CreateInstance<T>();

    private static void CreateCryptStream(ICryptoTransform transForm, byte[] encryptedBytes, MemoryStream memStream)
    {
        using CryptoStream stream = new(memStream, transForm, CryptoStreamMode.Write);
        stream.Write(encryptedBytes, 0, encryptedBytes.Length);
        stream.FlushFinalBlock();

    }

    private struct KeyIV
    {
        public byte[] key;
        public byte[] iv;
    }

    /// <summary>
    /// 用來加鹽的方法
    /// </summary>
    /// <param name="decryptString"></param>
    /// <param name="salt"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    private static KeyIV GetKeyIV(string decryptString, byte[] salt, SymmetricAlgorithm provider)
    {
        // http://stackoverflow.com/questions/18280379/what-is-the-default-padding-for-aescryptoserviceprovider
        Rfc2898DeriveBytes rfc = new(decryptString, salt);
        return new KeyIV
        {
            key = rfc.GetBytes(provider.KeySize / 8),
            iv = rfc.GetBytes(provider.BlockSize / 8)
        };
    }

    private static string Decrypt<T>(string encryptedString, string decryptString, string salt, Func<string, byte[], SymmetricAlgorithm, KeyIV> keyGetter)
    {
        byte[] byteSalt = salt == null ? Encoding.ASCII.GetBytes(DEFAULT_SALT) : Encoding.ASCII.GetBytes(salt);
        byte[] encryptedBytes = encryptedString.HexStringToByteArray();

        SymmetricAlgorithm? provider = CryptoProviderFactory<T>() as SymmetricAlgorithm;
        KeyIV keyIV = keyGetter.Invoke(decryptString, byteSalt, provider);

        ICryptoTransform transform = provider.CreateDecryptor(keyIV.key, keyIV.iv);

        string? result = null;

        using (MemoryStream memStream = new())
        {
            CreateCryptStream(transform, encryptedBytes, memStream);
            result = string.Concat(Encoding.Unicode.GetChars(memStream.ToArray()));
        }
        return result;
    }

    private static string Encrypt<T>(string unEncryptedString, string decryptString, string salt, Func<string, byte[], SymmetricAlgorithm, KeyIV> keyGetter)
    {
        byte[] byteSalt = salt == null ? Encoding.ASCII.GetBytes(DEFAULT_SALT) : Encoding.ASCII.GetBytes(salt);
        byte[] unEncryptedBytes = Encoding.Unicode.GetBytes(unEncryptedString);
        SymmetricAlgorithm? provider = CryptoProviderFactory<T>() as SymmetricAlgorithm;
        KeyIV keyIV = keyGetter.Invoke(decryptString, byteSalt, provider);

        ICryptoTransform transForm = provider.CreateEncryptor(keyIV.key, keyIV.iv);

        string? result = null;

        using (MemoryStream memStream = new())
        {
            CreateCryptStream(transForm, unEncryptedBytes, memStream);
            result = memStream.ToArray().ByteArrayToHexString();
        }
        return result;
    }

    /// <summary>
    /// RC2(128Bits)加密(用預設的鹽巴)
    /// </summary>
    /// <param name="unEncryptedString">欲加密的字串</param>
    /// <param name="decryptString">密鑰(ascii 8 bytes)</param>
    /// <returns></returns>
    public static string RC2Encrypt(string unEncryptedString, string decryptString) => RC2Encrypt(unEncryptedString, decryptString, DEFAULT_SALT);

    /// <summary>
    /// RC2(128Bits)加密
    /// </summary>
    /// <param name="unEncryptedString">欲加密的字串</param>
    /// <param name="decryptString">密鑰(ascii 16 bytes)</param>
    /// <param name="salt">鹽巴(至少 8 bytes)</param>
    /// <returns></returns>
    public static string RC2Encrypt(string unEncryptedString, string decryptString, string salt) => Encrypt<RC2>(unEncryptedString, decryptString, salt, GetKeyIV);

    /// <summary>
    /// RC2(128Bits)解密(用預設的鹽巴)
    /// </summary>
    /// <param name="encryptedString">欲解密的字串</param>
    /// <param name="decryptString">密鑰(ascii 16 bytes)</param>
    /// <returns></returns>
    public static string RC2Decrypt(string encryptedString, string decryptString) => RC2Decrypt(encryptedString, decryptString, DEFAULT_SALT);

    /// <summary>
    /// RC2(128Bits)解密
    /// </summary>
    /// <param name="encryptedString">欲解密的字串</param>
    /// <param name="decryptString">密鑰(ascii 16 bytes)</param>
    /// <param name="salt">鹽巴(至少 8 bytes)</param>
    /// <returns></returns>
    public static string RC2Decrypt(string encryptedString, string decryptString, string salt) => Decrypt<RC2>(encryptedString, decryptString, salt, GetKeyIV);

}

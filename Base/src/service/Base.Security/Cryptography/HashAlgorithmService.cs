using System.Security.Cryptography;
using System.Text;
using Base.Domain.Options.Cryptography;
using Base.Domain.SeedWorks;
using Base.Infrastructure.Interface.Security.Cryptography;
using Base.Infrastructure.Toolkits.Extensions;
using Base.Infrastructure.Toolkits.Resources;

namespace Base.Security.Cryptography;

public class HashAlgorithmService : BaseService, IHashAlgorithmService
{
    private readonly HashAlgorithm defaultHasAlgorithm;

    public HashAlgorithmService(HashAlgorithmSetting setting)
    {
        // 取得 HashType 和 HashTypeList
        string hashType = setting.HashType;

        // 系統支援的 Hash 演算法
        string[] HashAlgorithmList = SystemResource.HashAlgorithm.SplitColumns();
        if (!HashAlgorithmList.Any(x => x == hashType))
        {
            throw new ArgumentException(MessageResource.ArgumentExceptionMessage.SetCustomerMessage("演算法設定異常"), nameof(hashType));
        }

        // 設定 Hash 演算法
        defaultHasAlgorithm = hashType switch
        {
            "SHA1" => SHA1.Create(),
            "SHA256" => SHA256.Create(),
            "SHA384" => SHA384.Create(),
            "SHA512" => SHA512.Create(),
            "MD5" => MD5.Create(),
            _ => SHA1.Create(), // 若無法匹配，預設使用 SHA1
        };
    }

    /// <summary>
    /// 進行 Hash 計算
    /// </summary>
    /// <param name="text">字串</param>
    /// <returns>Hash 字串</returns>
    public string Hash(string text)
    {
        byte[] bytes = Encoding.Unicode.GetBytes(text);
        return Hash(bytes, defaultHasAlgorithm).ByteArrayToHexString();
    }

    /// <summary>
    /// 進行 SHA1 Hash 計算 (對外)
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public string SHA1Hash(string text) => SHA1HashStatic(text);

    /// <summary>
    /// 進行 SHA1 Hash 計算
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    internal static string SHA1HashStatic(string text)
    {
        byte[] bytes = Encoding.Unicode.GetBytes(text);
        using SHA1 sha1 = SHA1.Create();
        return Hash(bytes, sha1).ByteArrayToHexString();
    }

    /// <summary>
    /// 進行 SHA256 Hash 計算 (對外)
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public string SHA256Hash(string text) => SHA256HashStatic(text);

    /// <summary>
    /// 進行 SHA256 Hash 計算
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    internal static string SHA256HashStatic(string text)
    {
        byte[] bytes = Encoding.Unicode.GetBytes(text);

        using SHA256 sha256 = SHA256.Create();
        return Hash(bytes, sha256).ByteArrayToHexString();
    }

    /// <summary>
    /// 進行 SHA384 Hash 計算 (對外)
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public string SHA384Hash(string text) => SHA384HashStatic(text);

    /// <summary>
    /// 進行 SHA384 Hash 計算
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    internal static string SHA384HashStatic(string text)
    {
        byte[] bytes = Encoding.Unicode.GetBytes(text);
        using SHA384 sha384 = SHA384.Create();
        return Hash(bytes, sha384).ByteArrayToHexString();
    }

    /// <summary>
    /// 進行 SHA512 Hash 計算 (對外)
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public string SHA512Hash(string text) => SHA512HashStatic(text);

    /// <summary>
    /// 進行 SHA512 Hash 計算
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    internal static string SHA512HashStatic(string text)
    {
        byte[] bytes = Encoding.Unicode.GetBytes(text);
        using SHA512 sha512 = SHA512.Create();
        return Hash(bytes, sha512).ByteArrayToHexString();
    }

    /// <summary>
    /// 進行 MD5 Hash 計算 (對外)
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public string MD5Hash(string text) => MD5HashStatic(text);

    /// <summary>
    /// 進行 MD5 Hash 計算
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    internal static string MD5HashStatic(string text)
    {
        byte[] bytes = Encoding.Unicode.GetBytes(text);
        using MD5 md5 = MD5.Create();
        return Hash(bytes, md5).ByteArrayToHexString();
    }

    /// <summary>
    /// 進行 Hash 計算
    /// </summary>
    /// <param name="source">來源資料</param>
    /// <param name="hashAlgorithm">雜湊法</param>
    /// <returns>雜湊後資料</returns>
    private static byte[] Hash(byte[] source, HashAlgorithm hashAlgorithm) => hashAlgorithm.ComputeHash(source);

    public override void Dispose(bool disposing)
    {
        defaultHasAlgorithm.Dispose();
        base.Dispose(disposing);
    }
}

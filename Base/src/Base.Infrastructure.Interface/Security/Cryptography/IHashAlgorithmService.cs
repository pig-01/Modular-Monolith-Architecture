namespace Base.Infrastructure.Interface.Security.Cryptography;

public interface IHashAlgorithmService
{
    /// <summary>
    /// 進行 Hash 計算
    /// </summary>
    /// <param name="text">字串</param>
    /// <returns>Hash 字串</returns>
    string Hash(string text);

    /// <summary>
    /// 進行 SHA1 Hash 計算
    /// </summary>
    /// <param name="text">字串</param>
    /// <returns>Hash 字串</returns>
    string SHA1Hash(string text);

    /// <summary>
    /// 進行 SHA256 Hash 計算
    /// </summary>
    /// <param name="text">字串</param>
    /// <returns>Hash 字串</returns>
    string SHA256Hash(string text);

    /// <summary>
    /// 進行 SHA384 Hash 計算
    /// </summary>
    /// <param name="text">字串</param>
    /// <returns>Hash 字串</returns>
    string SHA384Hash(string text);

    /// <summary>
    /// 進行 MD5 Hash 計算 (對外)
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    string MD5Hash(string text);

    /// <summary>
    /// 進行 SHA512 Hash 計算 (對外)
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    string SHA512Hash(string text);
}

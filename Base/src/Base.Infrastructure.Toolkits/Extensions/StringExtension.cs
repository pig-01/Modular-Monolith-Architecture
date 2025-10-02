using System.Runtime.InteropServices;
using System.Security;
using System.Text.Json;

namespace Base.Infrastructure.Toolkits.Extensions;

public static class StringExtension
{
    /// <summary>
    /// 將字串轉換為指定型別的物件
    /// </summary>
    /// <typeparam name="T">指定型別</typeparam>
    /// <param name="json">json字串</param>
    /// <returns>指定型別</returns>
    public static T? FromJson<T>(this string json) => JsonSerializer.Deserialize<T>(json, JsonExtension.Settings);

    /// <summary>
    /// 將物件轉換為 JSON 字串
    /// </summary>
    /// <param name="model">物件</param>
    /// <returns>JSON 字串</returns>
    public static string ToJson<T>(this T model) => JsonSerializer.Serialize(model, JsonExtension.Settings);

    /// <summary>
    /// 判斷字串是否為 Null 或 Empty
    /// </summary>
    /// <param name="str">字串</param>
    /// <returns>boolean</returns>
    public static bool IsNullOrEmpty(this string? str) => string.IsNullOrEmpty(str);

    /// <summary>
    /// 判斷字串是否為 Null 或 WhiteSpace
    /// </summary>
    /// <param name="str">字串</param>
    /// <returns>boolean</returns>
    public static bool IsNullOrWhiteSpace(this string? str) => string.IsNullOrWhiteSpace(str);

    /// <summary>
    /// 轉換nullable string到string,如空值則回傳空字串
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string NullToEmpty(this string? value) => value is null ? string.Empty : value.ToString();


    /// <summary>
    /// 多值欄位資料切分
    /// </summary>
    /// <param name="str">欄位資料</param>
    /// <returns>字串集合</returns>
    public static string[] SplitColumns(this string? str) => str == null ? [] : str.Split(',');

    /// <summary>
    /// 設定客製化訊息
    /// </summary>
    /// <remarks>
    /// 用於設定客製化訊息，例如：{message} 訊息
    /// 常用於錯誤訊息的客製化
    /// </remarks>
    /// <param name="str"></param>
    /// <param name="message"></param>
    /// <returns>組合後訊息</returns>
    public static string SetCustomerMessage(this string str, string message) =>
        str.Replace($"{{{nameof(message)}}}", EnvironmentExtension.IsDevelopment("ASPNETCORE_ENVIRONMENT") ? message : "", StringComparison.Ordinal);

    /// <summary>
    /// 安全字串轉成一般字串
    /// </summary>
    /// <param name="secureString">安全字串</param>
    /// <returns>解密後的字串</returns>
    public static string? ToRealString(this SecureString secureString)
    {
        IntPtr intPtr = IntPtr.Zero;
        try
        {
            intPtr = Marshal.SecureStringToGlobalAllocUnicode(secureString);
            return Marshal.PtrToStringUni(intPtr);
        }
        finally
        {
            Marshal.ZeroFreeGlobalAllocUnicode(intPtr);
        }
    }

    /// <summary>
    /// 轉成安全字串
    /// </summary>
    /// <param name="text">字串</param>
    /// <returns>安全字串</returns>
    public static SecureString ToSecureString(this string text)
    {
        SecureString secureString = new();
        if (string.IsNullOrEmpty(text))
        {
            return secureString;
        }

        foreach (char c in text)
        {
            secureString.AppendChar(c);
        }

        return secureString;
    }


    /// <summary>
    /// Byte 陣列轉成 Hex 字串。
    /// </summary>
    /// <param name="byteArray">Byte 陣列</param>
    /// <returns>Hex 字串</returns>
    public static string ByteArrayToHexString(this byte[] byteArray)
    {
        string result = "";
        foreach (byte b in byteArray)
        {
            string temp = Convert.ToByte(b).ToString("x");
            if (temp.Length < 2) temp = "0" + temp;
            result += temp;
        }
        return result;
    }

    /// <summary>
    /// Hex 字串轉成 Byte 陣列。
    /// </summary>
    /// <param name="hexString">Hex 字串</param>
    /// <returns>轉換後的 Byte 陣列</returns>
    public static byte[] HexStringToByteArray(this string hexString)
    {
        int len = hexString.Length;
        byte[] result = new byte[len / 2];
        for (int i = 0; i < len; i += 2)
        {
            result[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
        }
        return result;
    }

    public static bool IpCompare(this string mask, string target)
    {
        List<string> list = [.. mask.Split('.')];
        List<string> list2 = [.. target.Split('.')];
        if (list.Count != 4 || list2.Count != 4)
        {
            return mask == target;
        }

        for (int i = 0; i < 4; i++)
        {
            string s = list[i].NullToEmpty();
            string text = list2[i].NullToEmpty();
            int num = int.Parse(s);
            if (!int.TryParse(text, out int result))
            {
                if (i == 3 && text.IndexOf(':') > 0)
                {
                    if (!int.TryParse(text.Split(":".ToCharArray())[0], out result))
                    {
                        result = 256;
                    }
                }
                else
                {
                    result = 256;
                }
            }

            if ((num & result) != num)
            {
                return false;
            }
        }

        return true;
    }
}
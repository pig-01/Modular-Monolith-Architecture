namespace Base.Infrastructure.Toolkits.Utilities;

public static class HttpUtility
{
    public static string UrlEncode(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }
        return Uri.EscapeDataString(value).Replace("%20", "+");
    }

    public static string UrlDecode(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }
        return Uri.UnescapeDataString(value.Replace("+", "%20"));
    }
}

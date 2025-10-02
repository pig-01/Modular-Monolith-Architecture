
namespace Base.Infrastructure.Toolkits.Extensions;

public static class FunctionExtension
{
    /// <summary>
    /// 提供一個通用的異常處理擴展方法，支持同步和非同步方法
    /// </summary>
    /// <typeparam name="T">返回值類型</typeparam>
    /// <param name="func">要執行的方法</param>
    /// <param name="onException">自定義異常處理邏輯</param>
    /// <returns>執行結果</returns>
    public static T TryCatch<T>(this Func<T> func, Func<Exception, T> onException)
    {
        try
        {
            return func();
        }
        catch (Exception ex)
        {
            return onException(ex);
        }
    }

    /// <summary>
    /// 非同步方法的異常處理擴展
    /// </summary>
    public static async Task<T> TryCatch<T>(this Func<Task<T>> func, Func<Exception, T> onException)
    {
        try
        {
            return await func();
        }
        catch (Exception ex)
        {
            return onException(ex);
        }
    }

    /// <summary>
    /// 帶默認值的異常處理擴展方法
    /// </summary>
    public static T TryCatchWithDefault<T>(this Func<T> func, T defaultValue = default)
    {
        try
        {
            return func();
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// 非同步帶默認值的異常處理擴展方法
    /// </summary>
    public static async Task<T> TryCatchWithDefault<T>(this Func<Task<T>> func, T defaultValue = default)
    {
        try
        {
            return await func();
        }
        catch
        {
            return defaultValue;
        }
    }
}

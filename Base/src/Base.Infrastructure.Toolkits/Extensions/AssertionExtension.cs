namespace Base.Infrastructure.Toolkits.Extensions;

public static class Assertion
{
    /// <summary>
    /// 預期輸入字串「不可為」 空 或 null，否則出 Exception
    /// </summary>
    /// <param name="str"></param>
    /// <param name="exception"></param>
    public static void NotEmpty(string str, Exception exception)
    {
        if (string.IsNullOrEmpty(str))
        {
            throw exception;
        }
    }

    /// <summary>
    /// 預期輸入的條件「為」 true ，否則出 Exception
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="exception"></param>
    public static void IsTrue(bool expression, Exception exception)
    {
        if (!expression)
        {
            throw exception;
        }
    }

    /// <summary>
    /// 預期輸入條件「為」 false ，否則出 Exception
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="exception"></param>
    public static void IsNotTrue(bool expression, Exception exception)
    {
        if (expression)
        {
            throw exception;
        }
    }

    /// <summary>
    /// 預期輸入值「不可為」 null ，否則出 Exception
    /// </summary>
    /// <param name="target"></param>
    /// <param name="exception"></param>
    public static void IsNotNull(object target, Exception exception)
    {
        if (target == null) throw exception;
    }

    /// <summary>
    /// 預期輸入值「不可為」 null ，否則出 Exception
    /// </summary>
    /// <param name="target"></param>
    /// <param name="exception"></param>
    public static T IsNotNull<T>(this T? target, Exception exception) where T : class
    {
        if (target == null)
        {
            throw exception;  // 如果 target 是 null，丟出例外
        }

        // 返回非 nullable 型別，透過 null-forgiving operator 保證 target 不為 null
        return target!;
    }

    /// <summary>
    /// 預期輸入值「為」 null ，否則出 Exception
    /// </summary>
    /// <param name="target"></param>
    /// <param name="exception"></param>
    public static void IsNull(object target, Exception exception)
    {
        if (target != null) throw exception;
    }


}

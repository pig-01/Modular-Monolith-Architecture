namespace Base.Infrastructure.Toolkits.Extensions;

public static class ArrayExtension
{
    /// <summary>
    /// 判斷陣列是否為 Null 或 Empty
    /// </summary>
    /// <param name="array">陣列</param>
    /// <returns>如果陣列為 Null 或 Empty，則返回 true；否則返回 false。</returns>
    public static bool IsNullOrEmpty<T>(this T[] array) => array == null || array.Length == 0;

    /// <summary>
    /// 判斷陣列是否為 Null 或 Empty
    /// </summary>
    /// <param name="array">陣列</param>
    /// <returns>如果陣列為 Null 或 Empty，則返回 true；否則返回 false。</returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> array) => array == null || !array.Any();

    /// <summary>
    /// 判斷陣列中資料是否相同
    /// </summary>
    /// <typeparam name="T">陣列型別</typeparam>
    /// <param name="source">來源陣列</param>
    /// <param name="target">目標陣列</param>
    /// <returns>如果兩個陣列的資料相同，則返回 true；否則返回 false。</returns>
    public static bool Equals<T>(this T[] source, T[] target) where T : IEquatable<T>
    {
        // 檢查兩個陣列的長度是否相同
        if (source.Length != target.Length)
        {
            return false;
        }

        // 遍歷每個字節進行比對
        for (int i = 0; i < source.Length; i++)
        {
            if (!source[i].Equals(target[i]))
            {
                return false;
            }
        }

        // 如果所有字節都相同，則返回 true
        return true;
    }
}
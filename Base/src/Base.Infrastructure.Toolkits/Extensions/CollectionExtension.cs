namespace Base.Infrastructure.Toolkits.Extensions;

public static class CollectionExtension
{
    public static void AddRange<T>(this ICollection<T> destination,
                                   IEnumerable<T> source)
    {
        if (destination is List<T> list)
        {
            list.AddRange(source);
        }
        else
        {
            foreach (T item in source)
            {
                destination.Add(item);
            }
        }
    }
}

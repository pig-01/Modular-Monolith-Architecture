using System.ComponentModel.DataAnnotations;
using System.Reflection;


namespace DataHub.Domain.SeedWork;

public abstract class Enumerstion : IComparable
{
    [Required]
    public string Name { get; private set; }
    public int Id { get; private set; }

    protected Enumerstion(int id, string name) => (Id, Name) = (id, name);

    public override string ToString() => Name;

    public static IEnumerable<T> GetAll<T>() where T : Enumerstion =>
        typeof(T).GetFields(BindingFlags.Public |
                            BindingFlags.Static |
                            BindingFlags.DeclaredOnly)
                    .Select(f => f.GetValue(null))
                    .Cast<T>();

    public override bool Equals(object? obj)
    {
        if (obj is not Enumerstion otherValue)
        {
            return false;
        }

        bool typeMatches = GetType().Equals(obj?.GetType());
        bool valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static int AbsoluteDifference(Enumerstion firstValue, Enumerstion secondValue) => Math.Abs(firstValue.Id - secondValue.Id);

    public static T FromValue<T>(int value) where T : Enumerstion
    {
        T matchingItem = Parse<T, int>(value, "value", item => item.Id == value);
        return matchingItem;
    }

    public static T FromName<T>(string name) where T : Enumerstion
    {
        T matchingItem = Parse<T, string>(name, "name", item => item.Name == name);
        return matchingItem;
    }

    private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumerstion
    {
        T? matchingItem = GetAll<T>().FirstOrDefault(predicate);

        if (matchingItem == null)
            throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");

        return matchingItem;
    }

    public int CompareTo(object? obj) =>
        obj == null ? throw new ArgumentNullException(nameof(obj)) : Id.CompareTo(((Enumerstion)obj).Id);
    public static bool operator ==(Enumerstion left, Enumerstion right)
    {
        if (left is null)
            return right is null;

        return left.Equals(right);
    }

    public static bool operator !=(Enumerstion left, Enumerstion right) => !(left == right);

    public static bool operator <(Enumerstion left, Enumerstion right) => left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(Enumerstion left, Enumerstion right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(Enumerstion left, Enumerstion right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(Enumerstion left, Enumerstion right) => left is null ? right is null : left.CompareTo(right) >= 0;
}

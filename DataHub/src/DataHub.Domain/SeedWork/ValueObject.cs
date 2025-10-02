namespace DataHub.Domain.SeedWork;

public abstract class ValueObject
{
    protected static bool EqualsOperator(ValueObject left, ValueObject right)
    {
        if (left is null ^ right is null)
        {
            return false;
        }

        return left is null || left.Equals(right);
    }

    protected static bool NotEqualsOperator(ValueObject left, ValueObject right) => !EqualsOperator(left, right);

    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        ValueObject other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode() => GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);

    public ValueObject? GetCopy() => MemberwiseClone() as ValueObject;
}

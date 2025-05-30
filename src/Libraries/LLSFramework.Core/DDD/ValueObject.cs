namespace LLSFramework.Core.DDD;

/// <summary>
/// Represents a base class for value objects in Domain-Driven Design (DDD).
/// Value objects are immutable and compared based on their property values rather than identity.
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// Compares two value objects for equality using their equality components.
    /// </summary>
    /// <param name="left">The first value object.</param>
    /// <param name="right">The second value object.</param>
    /// <returns>True if both value objects are equal; otherwise, false.</returns>
    protected static bool EqualOperator(ValueObject left, ValueObject right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    /// <summary>
    /// Compares two value objects for inequality.
    /// </summary>
    /// <param name="left">The first value object.</param>
    /// <param name="right">The second value object.</param>
    /// <returns>True if the value objects are not equal; otherwise, false.</returns>
    protected static bool NotEqualOperator(ValueObject left, ValueObject right)
    {
        return !EqualOperator(left, right);
    }

    /// <summary>
    /// Returns the atomic values that define the equality of the value object.
    /// Derived classes must override this to provide their equality components.
    /// </summary>
    /// <returns>An enumerable of the components used for equality comparison.</returns>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    /// <summary>
    /// Determines whether the specified object is equal to the current value object.
    /// </summary>
    /// <param name="obj">The object to compare with the current value object.</param>
    /// <returns>True if the objects are equal; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType()) return false;

        var other = (ValueObject)obj;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    /// <summary>
    /// Returns a hash code based on the value object's equality components.
    /// </summary>
    /// <returns>A hash code for the value object.</returns>
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Aggregate(1, (current, obj) => current * 23 + (obj?.GetHashCode() ?? 0));
    }

    /// <summary>
    /// Equality operator for value objects.
    /// </summary>
    public static bool operator ==(ValueObject one, ValueObject two)
    {
        return EqualOperator(one, two);
    }

    /// <summary>
    /// Inequality operator for value objects.
    /// </summary>
    public static bool operator !=(ValueObject one, ValueObject two)
    {
        return NotEqualOperator(one, two);
    }
}
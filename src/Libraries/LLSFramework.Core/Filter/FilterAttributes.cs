namespace LLSFramework.Core.Filter;

/// <summary>
/// Indicates that the property is computed and should not be used for filtering in queries.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class FilterComputedAttribute : Attribute;

/// <summary>
/// Indicates that the property should be filtered using a "contains" (substring) operation.
/// Typically used for string properties.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class FilterContainsAttribute : Attribute;

/// <summary>
/// Indicates that the property should be filtered using a "greater than or equal" comparison for DateTime values.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class FilterDateTimeGreaterThanOrEqualAttribute : Attribute;

/// <summary>
/// Indicates that the property should be filtered using a "less than or equal" comparison for DateTime values.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class FilterDateTimeLessThanOrEqualAttribute : Attribute;

/// <summary>
/// Indicates that the property is an enum parameter for filtering.
/// Allows specifying whether the enum should be bound as an integer or as a string.
/// </summary>
/// <param name="bindAsInteger">If true, the enum is bound as an integer; otherwise, as a string.</param>
[AttributeUsage(AttributeTargets.Property)]
public class FilterEnumParameterAttribute(bool bindAsInteger = true) : Attribute
{
    /// <summary>
    /// Gets a value indicating whether the enum should be bound as an integer.
    /// </summary>
    public bool BindAsInteger { get; } = bindAsInteger;
}

/// <summary>
/// Indicates that the property should be filtered using an equality comparison.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class FilterEqualsAttribute : Attribute;

/// <summary>
/// Specifies that the property should be included as a query string parameter in filter operations.
/// Allows specifying a custom parameter name.
/// </summary>
/// <param name="name">The custom name for the query string parameter. If null, the property name is used.</param>
[AttributeUsage(AttributeTargets.Property)]
public class FilterQueryStringParameterAttribute(string? name = null) : Attribute
{
    /// <summary>
    /// Gets the custom name for the query string parameter.
    /// </summary>
    public string? Name { get; } = name;
}

/// <summary>
/// Specifies an underlying name for the property, typically used for mapping to a different field or column in the data source.
/// </summary>
/// <param name="underlyingName">The underlying name to use for filtering or mapping.</param>
[AttributeUsage(AttributeTargets.Property)]
public class FilterUnderlyingNameAttribute(string underlyingName) : Attribute
{
    /// <summary>
    /// Gets the underlying name for the property.
    /// </summary>
    public string UnderlyingName { get; } = underlyingName;
}

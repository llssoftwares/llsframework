namespace LLSFramework.Core.Parsers;

/// <summary>
/// Provides generic parsing utilities for converting string values to various types, including primitives, enums, and collections.
/// </summary>
public static class GenericParser
{
    /// <summary>
    /// Parses a string value into the specified type <typeparamref name="T"/>.
    /// Supports primitive types, nullable types, enums, lists of enums, and lists of (enum, string) tuples.
    /// Returns the default value of <typeparamref name="T"/> if parsing fails.
    /// </summary>
    /// <typeparam name="T">The target type to parse to.</typeparam>
    /// <param name="value">The string value to parse.</param>
    /// <returns>The parsed value as type <typeparamref name="T"/>, or default if parsing fails.</returns>
    public static T? Parse<T>(string value)
    {
        var underlyingType = Nullable.GetUnderlyingType(typeof(T));

        // Handle int and nullable int
        if (typeof(T) == typeof(int) || typeof(T) == typeof(int?))
            return int.TryParse(value, out var valueAsInt) ? (T)(object)valueAsInt : default;
        // Handle decimal and nullable decimal
        else if (typeof(T) == typeof(decimal) || typeof(T) == typeof(decimal?))
        {
            return decimal.TryParse(value, CultureInfo.InvariantCulture, out var valueAsDecimal) ? (T)(object)valueAsDecimal : default;
        }
        // Handle DateTime and nullable DateTime
        else if (typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTime?))
        {
            return DateTime.TryParse(value, out var valueAsDateTime) ? (T)(object)valueAsDateTime : default;
        }
        // Handle bool and nullable bool
        else if (typeof(T) == typeof(bool) || typeof(T) == typeof(bool?))
        {
            return bool.TryParse(value, out var valueAsBool) ? (T)(object)valueAsBool : default;
        }
        // Handle double and nullable double
        else if (typeof(T) == typeof(double) || typeof(T) == typeof(double?))
        {
            return double.TryParse(value, CultureInfo.InvariantCulture, out var valueAsDouble) ? (T)(object)valueAsDouble : default;
        }
        // Handle Guid and nullable Guid
        if (typeof(T) == typeof(Guid) || typeof(T) == typeof(Guid?))
            return Guid.TryParse(value, out var valueAsGuid) ? (T)(object)valueAsGuid : default;
        // Handle nullable enums
        else if (underlyingType != null)
        {
            return (T)EnumExtensions.Parse(underlyingType, value);
        }
        // Handle enums
        else if (typeof(T).IsEnum)
        {
            return (T)EnumExtensions.Parse(typeof(T), value);
        }
        // Handle lists of enums (e.g., List<MyEnum>)
        else if (EnumExtensions.IsListOfEnum(typeof(T)))
        {
            var splitValues = value.ToString().Split(',');

            var enumType = typeof(T).GetGenericArguments()[0];
            var enumValues = Activator.CreateInstance(typeof(List<>).MakeGenericType(enumType))!;
            var addMethod = enumValues.GetType().GetMethod("Add")!;

            foreach (var splitValue in splitValues)
            {
                var parsedEnum = EnumExtensions.Parse(enumType, splitValue);
                addMethod.Invoke(enumValues, [parsedEnum]);
            }

            return (T)enumValues;
        }
        // Handle lists of (enum, string) tuples (e.g., List<(MyEnum, string)>)
        else if (EnumExtensions.IsListOfTupleOfEnumString(typeof(T)))
        {
            var splitValues = value.ToString().Split(',');

            var genericType = typeof(T).GetGenericArguments()[0];

            var enumType = genericType.GetGenericArguments()[0];

            var result = Activator.CreateInstance<T>()!;

            var addMethod = result.GetType().GetMethod("Add")!;

            foreach (var splitValue in splitValues)
            {
                var parsedEnum = EnumExtensions.Parse(enumType, splitValue);

                var tuple = Activator.CreateInstance(typeof(ValueTuple<,>).MakeGenericType(enumType, typeof(string)), [parsedEnum, string.Empty]);

                addMethod.Invoke(result, [tuple]);
            }

            return (T)(object)result;
        }

        // Fallback: try to cast the string to the target type
        try
        {
            return (T)(object)value.ToString();
        }
        catch (Exception)
        {
            return default!;
        }
    }
}


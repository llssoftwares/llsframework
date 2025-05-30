namespace LLSFramework.Core.Extensions;

/// <summary>
/// Provides extension methods for working with enums, including description retrieval, parsing, and type checks.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Gets the <see cref="DescriptionAttribute"/> value of an enum member, or its name if no description is set.
    /// </summary>
    /// <param name="value">The enum value.</param>
    /// <returns>The description or name of the enum value.</returns>
    public static string GetDescription(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());
        if (fieldInfo == null) return value.ToString();

        var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0
            ? attributes[0].Description
            : value.ToString();
    }

    /// <summary>
    /// Gets the <see cref="DescriptionAttribute"/> value of a nullable enum member, or an empty string if null.
    /// </summary>
    /// <param name="value">The nullable enum value.</param>
    /// <returns>The description or an empty string if the value is null.</returns>
    public static string GetDescriptionNullable(this Enum? value)
    {
        return value != null
            ? value.GetDescription()
            : string.Empty;
    }

    /// <summary>
    /// Returns a list of tuples containing all enum values and their descriptions for a given enum type.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <returns>A list of (Value, Description) tuples for the enum.</returns>
    public static List<(T Value, string Description)> ToTupleList<T>()
    {
        var enumType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        var values = Enum.GetValues(enumType).Cast<T>();
        var list = new List<(T Value, string Description)>();

        foreach (var value in values)
        {
            var stringValue = value?.ToString();
            if (stringValue == null) continue;

            var memberInfo = enumType.GetMember(stringValue);
            var descriptionAttribute = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false).OfType<DescriptionAttribute>().FirstOrDefault();

            if (value == null) continue;

            list.Add(new(value, descriptionAttribute?.Description ?? stringValue));
        }

        return list;
    }

    /// <summary>
    /// Parses a string to an enum value of the specified type, ignoring case.
    /// Throws an exception if parsing fails.
    /// </summary>
    /// <param name="type">The enum type.</param>
    /// <param name="value">The string value to parse.</param>
    /// <returns>The parsed enum value.</returns>
    /// <exception cref="ArgumentException">Thrown if the value cannot be parsed to the enum type.</exception>
    public static object Parse(Type type, string? value)
    {
        return Enum.TryParse(type, value, ignoreCase: true, out var enumValue)
                ? enumValue
                : throw new ArgumentException($"O valor '{value}' não pode ser convertido para a enumeração {type.Name}.");
    }

    /// <summary>
    /// Determines if a type is a generic list of an enum type.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type is a list of enums; otherwise, false.</returns>
    public static bool IsListOfEnum(Type type)
    {
        if (type.IsGenericType && typeof(IList).IsAssignableFrom(type))
        {
            var genericType = type.GetGenericArguments()[0];
            return genericType.IsEnum;
        }

        return false;
    }

    /// <summary>
    /// Determines if a type is a generic list of tuples where the first item is an enum and the second is a string.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type is a list of (enum, string) tuples; otherwise, false.</returns>
    public static bool IsListOfTupleOfEnumString(Type type)
    {
        if (!type.IsGenericType || !typeof(IList).IsAssignableFrom(type))
            return false;

        var genericType = type.GetGenericArguments()[0];

        if (!genericType.IsGenericType || genericType.GetGenericTypeDefinition() != typeof(ValueTuple<,>))
            return false;

        var tupleArguments = genericType.GetGenericArguments();

        return tupleArguments[0].IsEnum && tupleArguments[1] == typeof(string);
    }

    /// <summary>
    /// Gets all values of an enum type as a list.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <returns>A list of all enum values.</returns>
    public static List<T> GetAllValues<T>() where T : Enum
    {
        return [.. Enum.GetValues(typeof(T)).Cast<T>()];
    }

    /// <summary>
    /// Updates <paramref name="list1"/> to contain only the tuples from <paramref name="list2"/> whose enum values exist in <paramref name="list1"/>.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="list1">The list to update (filtered in place).</param>
    /// <param name="list2">The source list to filter from.</param>
    public static void BindWithMatchingEnumValues<T>(this List<(T, string)> list1, List<(T, string)> list2) where T : Enum
    {
        var enums = list1.Select(z => z.Item1).ToList();

        list1.Clear();

        list1.AddRange([.. list2.Where(x => enums.Contains(x.Item1))]);
    }
}
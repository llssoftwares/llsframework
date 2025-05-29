namespace LLSFramework.Core.Filter;

/// <summary>
/// Provides extension methods for extracting filter parameters from <see cref="EntityFilter"/> instances.
/// </summary>
public static class EntityFilterExtensions
{
    /// <summary>
    /// Extracts a list of key-value pairs representing filter parameters from the given filter model.
    /// Only properties decorated with <c>FilterQueryStringParameterAttribute</c> are considered.
    /// Handles special formatting for nullable DateTime, enum lists, and enum values.
    /// </summary>
    /// <typeparam name="T">The type of the filter, derived from <see cref="EntityFilter"/>.</typeparam>
    /// <param name="model">The filter model instance.</param>
    /// <returns>A list of (Key, Value) tuples representing the filter parameters.</returns>
    public static List<(string Key, object? Value)> GetParametersFromFilter<T>(this T model) where T : EntityFilter
    {
        var parameters = new List<(string Key, object? Value)>();

        // Iterate over all properties except those declared in the base EntityFilter class
        foreach (var property in typeof(T).GetProperties().Where(p => p.DeclaringType != typeof(EntityFilter)))
        {
            // Retrieve custom attributes for query string and enum parameter handling
            var filterQueryStringParamAttribute = property.GetCustomAttribute<FilterQueryStringParameterAttribute>();
            var filterEnumParamAttribute = property.GetCustomAttribute<FilterEnumParameterAttribute>();

            // Only process properties with the FilterQueryStringParameterAttribute
            if (filterQueryStringParamAttribute is null) continue;

            // Use the attribute's Name if provided, otherwise use the property name
            var key = !string.IsNullOrEmpty(filterQueryStringParamAttribute.Name)
                ? filterQueryStringParamAttribute.Name
                : property.Name;

            var value = property.GetValue(model, null);
            if (value == null) continue;

            // Special handling for nullable DateTime: format as date or datetime string
            if (property.PropertyType == typeof(DateTime?))
            {
                if (value is not null)
                {
                    var dateTimeValue = (DateTime)value;
                    value = dateTimeValue.Hour == 0 && dateTimeValue.Minute == 0 && dateTimeValue.Second == 0
                        ? ((DateTime)value).ToString("yyyy-MM-dd")
                        : (object)((DateTime)value).ToString("yyyy-MM-ddTHH:mm:ss");
                }
            }
            // Handle lists of (enum, string) tuples by joining enum values as integers
            else if (EnumExtensions.IsListOfTupleOfEnumString(property.PropertyType))
            {
                value = string.Join(",",
                    ((IEnumerable)value)
                    .Cast<object>()
                    .Select(item =>
                    {
                        var enumValue = item.GetType().GetField("Item1")!.GetValue(item);
                        return Convert.ToInt32(enumValue);
                    })
                );

            }
            // Handle enum parameters, supporting both lists and single values
            else if (filterEnumParamAttribute != null)
            {
                if (EnumExtensions.IsListOfEnum(value.GetType()))
                {
                    var enumType = value.GetType().GetGenericArguments()[0];

                    var enumerable = (IEnumerable)value;

                    List<string> enumValues = [];

                    foreach (var item in enumerable)
                    {
                        if (filterEnumParamAttribute.BindAsInteger)
                            enumValues.Add(Convert.ToInt32(item).ToString());
                        else
                        {
                            enumValues.Add(item!.ToString()!);
                        }
                    }

                    value = enumValues.Count != 0
                        ? string.Join(",", enumValues)
                        : null;
                }
                else
                {
                    var enumUnderlyingType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                    if (enumUnderlyingType.IsEnum)
                    {
                        if (enumUnderlyingType == typeof(Nullable<>))
                        {
                            var nullableValue = value.GetType().GetProperty("Value")?.GetValue(value, null);

                            value = nullableValue != null
                                ? filterEnumParamAttribute.BindAsInteger
                                    ? Convert.ToInt32(nullableValue)
                                    : nullableValue
                                : null;
                        }
                        else
                        {
                            value = (int)value;
                        }
                    }
                }
            }

            parameters.Add((key, value));
        }

        return parameters;
    }
}
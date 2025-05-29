namespace LLSFramework.Core.Filter;

/// <summary>
/// Provides extension methods for applying dynamic filtering to <see cref="IQueryable{T}"/> sources
/// based on filter objects derived from <see cref="EntityFilter"/>.
/// </summary>
public static class FilterExtensions
{
    /// <summary>
    /// Dynamically applies filtering to an <see cref="IQueryable{T1}"/> based on the non-null properties
    /// of a filter object of type <typeparamref name="T2"/>. Supports various property types and custom filter attributes.
    /// </summary>
    /// <typeparam name="T1">The entity type being queried.</typeparam>
    /// <typeparam name="T2">The filter type, derived from <see cref="EntityFilter"/>.</typeparam>
    /// <param name="query">The queryable source to filter.</param>
    /// <param name="filter">The filter object containing filter criteria.</param>
    /// <returns>An <see cref="IQueryable{T1}"/> with the applied filters.</returns>
    public static IQueryable<T1> Filter<T1, T2>(this IQueryable<T1> query, T2? filter)
        where T1 : class
        where T2 : EntityFilter
    {
        if (filter == null) return query;

        // Get filter properties, excluding those from the base EntityFilter class
        var filterProperties = filter.GetType().GetProperties().Where(p => p.DeclaringType != typeof(EntityFilter));

        var t1Properties = typeof(T1).GetProperties();

        var parameterExpr = Expression.Parameter(typeof(T1), "x");
        Expression predicateBody = Expression.Constant(true, typeof(bool));

        foreach (var property in filterProperties)
        {
            var filterValue = property.GetValue(filter);

            if (filterValue == null) continue;

            var propertyName = property.Name;

            // Check for an attribute that specifies an alternative property name
            var underlyingNameAttribute = property.GetCustomAttribute<FilterUnderlyingNameAttribute>();

            if (underlyingNameAttribute?.UnderlyingName != null)
                propertyName = underlyingNameAttribute.UnderlyingName;

            // Skip if the entity does not have the property
            if (!t1Properties.Any(x => x.Name == propertyName)) continue;

            var propertyExpr = Expression.Property(parameterExpr, propertyName);

            // Handle int and int? properties
            if (property.PropertyType == typeof(int))
            {
                var filterExpr = Expression.Constant(filterValue, typeof(int));

                // Ensure both sides have the same nullability
                if (propertyExpr.Type == filterExpr.Type)
                {
                    var equalityExpr = Expression.Equal(propertyExpr, filterExpr);
                    predicateBody = Expression.AndAlso(predicateBody, equalityExpr);
                }
                else
                {
                    // Compare values directly if types differ
                    var valueExpr = Expression.Property(filterExpr, "Value");
                    var equalityExpr = Expression.Equal(propertyExpr, valueExpr);
                    predicateBody = Expression.AndAlso(predicateBody, equalityExpr);
                }

            }
            else if (property.PropertyType == typeof(int?))
            {
                if (((int?)filterValue).HasValue)
                {
                    var filterExpr = Expression.Constant(filterValue, typeof(int?));

                    if (propertyExpr.Type == filterExpr.Type)
                    {
                        var equalityExpr = Expression.Equal(propertyExpr, filterExpr);
                        predicateBody = Expression.AndAlso(predicateBody, equalityExpr);
                    }
                    else
                    {
                        var valueExpr = Expression.Property(filterExpr, "Value");
                        var equalityExpr = Expression.Equal(propertyExpr, valueExpr);
                        predicateBody = Expression.AndAlso(predicateBody, equalityExpr);
                    }
                }
            }
            // Handle Guid and Guid? properties
            else if (property.PropertyType == typeof(Guid))
            {
                var filterExpr = Expression.Constant(filterValue, typeof(Guid));

                if (propertyExpr.Type == filterExpr.Type)
                {
                    var equalityExpr = Expression.Equal(propertyExpr, filterExpr);
                    predicateBody = Expression.AndAlso(predicateBody, equalityExpr);
                }
                else
                {
                    var valueExpr = Expression.Property(filterExpr, "Value");
                    var equalityExpr = Expression.Equal(propertyExpr, valueExpr);
                    predicateBody = Expression.AndAlso(predicateBody, equalityExpr);
                }

            }
            else if (property.PropertyType == typeof(Guid?))
            {
                if (((Guid?)filterValue).HasValue)
                {
                    var filterExpr = Expression.Constant(filterValue, typeof(Guid?));

                    if (propertyExpr.Type == filterExpr.Type)
                    {
                        var equalityExpr = Expression.Equal(propertyExpr, filterExpr);
                        predicateBody = Expression.AndAlso(predicateBody, equalityExpr);
                    }
                    else
                    {
                        var valueExpr = Expression.Property(filterExpr, "Value");
                        var equalityExpr = Expression.Equal(propertyExpr, valueExpr);
                        predicateBody = Expression.AndAlso(predicateBody, equalityExpr);
                    }
                }
            }
            // Handle enum and nullable enum properties
            else if (property.PropertyType.IsEnum || (Nullable.GetUnderlyingType(property.PropertyType)?.IsEnum ?? false))
            {
                var enumType = property.PropertyType.IsEnum
                    ? property.PropertyType
                    : Nullable.GetUnderlyingType(property.PropertyType);

                if (enumType == null) continue;

                var enumValue = Enum.Parse(enumType, filterValue?.ToString() ?? string.Empty);

                if (propertyExpr.Type == typeof(int))
                {
                    enumValue = (int)enumValue;
                    enumType = typeof(int);
                }
                else if (propertyExpr.Type == typeof(int?))
                {
                    enumValue = (int)enumValue;
                    enumType = typeof(int?);
                }

                var filterExpr = Expression.Constant(enumValue, enumType);
                var equalityExpr = Expression.Equal(propertyExpr, filterExpr);
                predicateBody = Expression.AndAlso(predicateBody, equalityExpr);
            }
            // Handle lists of enums
            else if (EnumExtensions.IsListOfEnum(property.PropertyType))
            {
                var enumType = property.PropertyType.GetGenericArguments().First();
                if (enumType == null) continue;

                // Get the enum values from the filter
                var enumValues = (IEnumerable)filterValue;
                if (enumValues == null || !enumValues.Cast<object>().Any()) continue;

                // Build a Contains expression for the enum list
                var containsMethod = typeof(List<>).MakeGenericType(enumType).GetMethod("Contains")!;
                var filterEnumValues = Expression.Constant(enumValues, typeof(List<>).MakeGenericType(enumType));
                var containsExpr = Expression.Call(filterEnumValues, containsMethod, propertyExpr);
                predicateBody = Expression.AndAlso(predicateBody, containsExpr);
            }
            // Handle string properties with custom filter attributes
            else if (property.PropertyType == typeof(string))
            {
                var containsAttribute = property.GetCustomAttribute<FilterContainsAttribute>();
                var equalsAttribute = property.GetCustomAttribute<FilterEqualsAttribute>();

                if (containsAttribute != null)
                {
                    // Case-insensitive "contains" filter
                    var filterString = ((string)filterValue).ToLower();
                    var propertyStringExpr = Expression.Call(propertyExpr, "ToLower", null);
                    var filterExpr = Expression.Constant(filterString, typeof(string));
                    var filterStringExpr = Expression.Call(filterExpr, "ToLower", null);
                    var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)]);

                    if (containsMethod != null)
                    {
                        var containsExpr = Expression.Call(propertyStringExpr, containsMethod, filterStringExpr);
                        predicateBody = Expression.AndAlso(predicateBody, containsExpr);
                    }
                }
                else if (equalsAttribute != null)
                {
                    // Exact match filter
                    var filterExpr = Expression.Constant(filterValue, typeof(string));
                    var equalityExpr = Expression.Equal(propertyExpr, filterExpr);
                    predicateBody = Expression.AndAlso(predicateBody, equalityExpr);
                }
            }
            // Handle nullable DateTime with custom comparison attributes
            else if (property.PropertyType == typeof(DateTime?))
            {
                var dateTimeGreaterThanOrEqualAttribute = property.GetCustomAttribute<FilterDateTimeGreaterThanOrEqualAttribute>();
                var dateTimeLessThanOrEqualAttribute = property.GetCustomAttribute<FilterDateTimeLessThanOrEqualAttribute>();

                var filterExpr = Expression.Constant(filterValue, propertyExpr.Type);

                BinaryExpression? equalityExpr = null;

                if (dateTimeGreaterThanOrEqualAttribute != null)
                    equalityExpr = Expression.GreaterThanOrEqual(propertyExpr, filterExpr);
                else if (dateTimeLessThanOrEqualAttribute != null)
                    equalityExpr = Expression.LessThanOrEqual(propertyExpr, filterExpr);

                if (equalityExpr != null)
                    predicateBody = Expression.AndAlso(predicateBody, equalityExpr);
            }
        }

        // Build and apply the final predicate
        var lambdaExpr = Expression.Lambda<Func<T1, bool>>(predicateBody, parameterExpr);

        return query.Where(lambdaExpr);
    }
}
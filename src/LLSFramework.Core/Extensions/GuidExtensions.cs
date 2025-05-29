namespace LLSFramework.Core.Extensions;

/// <summary>
/// Provides extension methods for converting strings to <see cref="Guid"/> and nullable <see cref="Guid"/> values.
/// </summary>
public static class GuidExtensions
{
    /// <summary>
    /// Converts the specified string to a <see cref="Guid"/>.
    /// Returns <see cref="Guid.Empty"/> if the string is not a valid GUID.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>The parsed <see cref="Guid"/>, or <see cref="Guid.Empty"/> if parsing fails.</returns>
    public static Guid ToGuid(this string value)
    {
        return Guid.TryParse(value, out var result) ? result : Guid.Empty;
    }

    /// <summary>
    /// Converts the specified string to a nullable <see cref="Guid"/>.
    /// Returns <c>null</c> if the string is null or not a valid GUID.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>The parsed <see cref="Guid"/>, or <c>null</c> if parsing fails or the string is null.</returns>
    public static Guid? ToNullableGuid(this string? value)
    {
        return Guid.TryParse(value, out var result) ? result : null;
    }
}
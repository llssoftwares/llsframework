namespace LLSFramework.Core.Extensions;

/// <summary>
/// Provides extension methods for formatting <see cref="decimal"/> and nullable <see cref="decimal"/> values as currency strings.
/// </summary>
public static class DecimalExtensions
{
    /// <summary>
    /// Converts a nullable decimal value to a currency string using the specified culture.
    /// Returns null if the value is null.
    /// </summary>
    /// <param name="value">The nullable decimal value to format.</param>
    /// <param name="cultureName">The culture name for formatting (default: "pt-BR").</param>
    /// <returns>A currency-formatted string, or null if the value is null.</returns>
    public static string? ToCurrencyString(this decimal? value, string cultureName = "pt-BR")
    {
        return value?.ToCurrencyString(cultureName);
    }

    /// <summary>
    /// Converts a decimal value to a currency string using the specified culture.
    /// </summary>
    /// <param name="value">The decimal value to format.</param>
    /// <param name="cultureName">The culture name for formatting (default: "pt-BR").</param>
    /// <returns>A currency-formatted string.</returns>
    public static string ToCurrencyString(this decimal value, string cultureName = "pt-BR")
    {
        return value.ToString("C", new CultureInfo(cultureName));
    }
}
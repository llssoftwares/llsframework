namespace LLSFramework.Core.Extensions;

/// <summary>
/// Provides extension methods for string manipulation, such as applying masks and extracting numeric characters.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Applies a mask to the input string, replacing each '#' in the mask with the corresponding character from the input.
    /// Non-placeholder characters in the mask are inserted as-is.
    /// Example: value = "123456789", mask = "###.###.###-##" => "123.456.789-"
    /// </summary>
    /// <param name="value">The input string to be masked.</param>
    /// <param name="mask">The mask pattern, where '#' represents a character from the input.</param>
    /// <returns>The masked string, or the original value if null or whitespace.</returns>
    public static string? ApplyMask(this string? value, string mask)
    {
        if (string.IsNullOrWhiteSpace(value)) return value;

        var newValue = string.Empty;

        var position = 0;

        foreach (var t in mask)
        {
            if (t == '#')
            {
                if (value.Length > position)
                {
                    newValue += value[position];
                    position++;
                }
                else
                {
                    break;
                }
            }
            else
                if (value.Length > position)
                newValue += t;
            else break;
        }

        return newValue;
    }

    /// <summary>
    /// Returns a string containing only the numeric characters from the input string.
    /// </summary>
    /// <param name="value">The input string to filter.</param>
    /// <returns>A string with only numeric characters, or the original value if null or whitespace.</returns>
    public static string? NumericOnly(this string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? value
            : Regex.Replace(value, "[^0-9]", string.Empty);
    }
}
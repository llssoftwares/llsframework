namespace LLSFramework.Core.Extensions;

/// <summary>
/// Provides extension methods for <see cref="DateTime"/> and <see cref="DateTime?"/> to support
/// time zone conversion and human-readable time formatting.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Returns a human-readable string representing how much time has passed since the specified <see cref="DateTime"/>.
    /// Example: "há 5 minutos", "ontem", "há 1 ano".
    /// </summary>
    /// <param name="dateTime">The date and time to compare with the current time.</param>
    /// <returns>A string describing the elapsed time in Portuguese.</returns>
    public static string AsTimeAgo(this DateTime dateTime)
    {
        var timeSpan = DateTime.Now.Subtract(dateTime);

        return timeSpan.TotalSeconds switch
        {
            <= 60 => $"há {timeSpan.Seconds} segundos",
            _ => timeSpan.TotalMinutes switch
            {
                <= 1 => "há 1 minuto",
                < 60 => $"há {timeSpan.Minutes} minutos",
                _ => timeSpan.TotalHours switch
                {
                    <= 1 => "há 1 hora",
                    < 24 => $"há {timeSpan.Hours} horas",
                    _ => timeSpan.TotalDays switch
                    {
                        <= 1 => "ontem",
                        <= 30 => $"há {timeSpan.Days} dias",
                        <= 60 => "há um mês",
                        < 365 => $"há {timeSpan.Days / 30} meses",
                        <= 365 * 2 => "há 1 ano",
                        _ => $"há {timeSpan.Days / 365} anos"
                    }
                }
            }
        };
    }

    /// <summary>
    /// Converts a UTC <see cref="DateTime"/> to the specified time zone.
    /// </summary>
    /// <param name="dateTime">The UTC date and time to convert.</param>
    /// <param name="timeZoneId">The time zone identifier (default: "E. South America Standard Time").</param>
    /// <returns>The converted <see cref="DateTime"/> in the specified time zone.</returns>
    public static DateTime ToTimeZone(this DateTime dateTime, string timeZoneId = "E. South America Standard Time")
    {
        var timeZOneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

        return TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZOneInfo);
    }

    /// <summary>
    /// Converts a nullable UTC <see cref="DateTime"/> to the specified time zone.
    /// Returns null if the input is null.
    /// </summary>
    /// <param name="dateTime">The nullable UTC date and time to convert.</param>
    /// <param name="timeZoneId">The time zone identifier (default: "E. South America Standard Time").</param>
    /// <returns>The converted <see cref="DateTime"/> in the specified time zone, or null.</returns>
    public static DateTime? ToTimeZone(this DateTime? dateTime, string timeZoneId = "E. South America Standard Time")
    {
        if (!dateTime.HasValue) return null;

        var timeZOneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

        return TimeZoneInfo.ConvertTimeFromUtc(dateTime.Value, timeZOneInfo);
    }

    /// <summary>
    /// Converts a UTC <see cref="DateTime"/> to the specified time zone and formats it as a string.
    /// </summary>
    /// <param name="dateTime">The UTC date and time to convert and format.</param>
    /// <param name="timeZoneId">The time zone identifier (default: "E. South America Standard Time").</param>
    /// <param name="format">The date and time format string (default: "dd/MM/yyyy HH:mm:ss").</param>
    /// <returns>The formatted date and time string in the specified time zone.</returns>
    public static string ToTimeZoneString(this DateTime dateTime, string timeZoneId = "E. South America Standard Time", string format = "dd/MM/yyyy HH:mm:ss")
    {
        return dateTime.ToTimeZone(timeZoneId).ToString(format);
    }

    /// <summary>
    /// Converts a nullable UTC <see cref="DateTime"/> to the specified time zone and formats it as a string.
    /// Returns null if the input is null.
    /// </summary>
    /// <param name="dateTime">The nullable UTC date and time to convert and format.</param>
    /// <param name="timeZoneId">The time zone identifier (default: "E. South America Standard Time").</param>
    /// <param name="format">The date and time format string (default: "dd/MM/yyyy HH:mm:ss").</param>
    /// <returns>The formatted date and time string in the specified time zone, or null.</returns>
    public static string? ToTimeZoneString(this DateTime? dateTime, string timeZoneId = "E. South America Standard Time", string format = "dd/MM/yyyy HH:mm:ss")
    {
        return dateTime?.ToTimeZone(timeZoneId).ToString(format);
    }
}
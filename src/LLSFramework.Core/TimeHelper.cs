namespace LLSFramework.Core;

/// <summary>
/// Provides helper methods for working with time-related operations.
/// </summary>
public static class TimeHelper
{
    /// <summary>
    /// Generates a list of <see cref="TimeOnly"/> values representing all times in a 24-hour period,
    /// incremented by the specified number of minutes.
    /// </summary>
    /// <param name="incrementMinutes">The interval, in minutes, between each time value.</param>
    /// <returns>A list of <see cref="TimeOnly"/> objects for each increment in the day.</returns>
    public static List<TimeOnly> GetHours(int incrementMinutes)
    {
        var result = new List<TimeOnly>();
        var totalMinutes = 24 * 60;

        // Iterate through the day in increments, adding each time to the result list
        for (var minutes = 0; minutes < totalMinutes; minutes += incrementMinutes)
        {
            var hours = minutes / 60;
            var remainingMinutes = minutes % 60;
            var time = new TimeOnly(hours, remainingMinutes, 0);
            result.Add(time);
        }

        return result;
    }
}
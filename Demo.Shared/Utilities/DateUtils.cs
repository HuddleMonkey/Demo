using Demo.Shared.Constants;

namespace Demo.Shared.Utilities;

public static class DateUtils
{
    /// <summary>
    /// Checks if the date falls within the window of the start/end date.
    /// </summary>
    /// <param name="startDate">Optional start date of the window</param>
    /// <param name="endDate">Optional end date of the window</param>
    /// <param name="date">Date to check</param>
    /// <returns>True if the date is in the window</returns>
    public static bool IsDateInWindow(DateTime? startDate, DateTime? endDate, DateTime date)
    {
        if (!startDate.HasValue && !endDate.HasValue) return true;
        if (startDate.HasValue && !endDate.HasValue && startDate.Value <= date) return true;
        if (!startDate.HasValue && endDate.HasValue && endDate.Value >= date) return true;
        if (startDate.HasValue && endDate.HasValue && startDate.Value <= date && endDate.Value >= date) return true;

        return false;
    }

    /// <summary>
    /// Gets the availability based on the available from/until dates
    /// </summary>
    /// <param name="availableFrom">Available From</param>
    /// <param name="availableUntil">Available Until</param>
    /// <param name="today">Today's date</param>
    /// <returns>Availability</returns>
    public static Availability GetAvailability(DateTime? availableFrom, DateTime? availableUntil, DateTime today)
    {
        if (IsDateInWindow(availableFrom, availableUntil, today)) return Availability.Available;
        if (availableFrom.HasValue && availableFrom.Value.Date > today) return Availability.Upcoming;

        return Availability.Past;
    }

    /// <summary>
    /// Converts a DateTime in UTC to the time zone identified by the time zone id
    /// </summary>
    /// <param name="dateUtc">DateTime to convert</param>
    /// <param name="timeZoneId">Id of the time zone to convert the date to. If none is specified then the UTC date is returned as is.</param>
    /// <returns>Converted DateTime</returns>
    public static DateTime ConvertDateFromUtc(DateTime dateUtc, string? timeZoneId)
    {
        if (!string.IsNullOrWhiteSpace(timeZoneId))
        {
            try
            {
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                DateTime convertedDateTime = TimeZoneInfo.ConvertTimeFromUtc(dateUtc, timeZoneInfo);

                return convertedDateTime;
            }
            catch
            {
                return dateUtc;
            }
        }

        return dateUtc;
    }
}

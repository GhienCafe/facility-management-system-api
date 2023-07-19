namespace AppCore.Extensions;

public static class DatetimeExtension
{
    public static long NowSeconds()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public static long TotalSeconds(this DateTime dateTime)
    {
        var dateTimeOffset = new DateTimeOffset(dateTime);
        return dateTimeOffset.ToUnixTimeSeconds();
    }

    public static DateTime UtcNow()
    {
        return DateTime.UtcNow;
    }

    public static DateTimeOffset Now(string timeZoneId)
    {
        return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTimeOffset.UtcNow, timeZoneId);
    }

    public static DateTime ToLocal(this DateTime dateTime, string convertToTimeZone)
    {
        if (convertToTimeZone.IsNullOrEmpty())
            convertToTimeZone = "UTC";

        if (dateTime.Kind == DateTimeKind.Local)
            return dateTime;

        var sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById(convertToTimeZone);
        return TimeZoneInfo.ConvertTimeFromUtc(dateTime, sourceTimeZone);
    }

    public static DateTime ToUtc(this DateTime dateTime, string sourceTimeZoneId)
    {
        if (sourceTimeZoneId.IsNullOrEmpty())
            sourceTimeZoneId = "UTC";
        if (dateTime.Kind == DateTimeKind.Utc)
        {
            return dateTime;
        }

        var sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById(sourceTimeZoneId);
        return TimeZoneInfo.ConvertTimeToUtc(dateTime, sourceTimeZone);
    }
}
using System.Runtime.CompilerServices;

namespace Shared;

public static class DateTimeOffsetHelpers
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset NaiveConversionToTokyoTime(DateTime dateTime)
    {
        return new DateTimeOffset(dateTime.AddHours(-9), TimeSpan.Zero);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset InlineTimezoneLookupTokyo(DateTime dateTime)
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
        return TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZone);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset ToUtc(DateTimeOffset dateTimeInJapanTimeZone)
    {
        return dateTimeInJapanTimeZone.ToUniversalTime();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset PassInTimezone(DateTime dateTimeInJapanTimeZone, TimeZoneInfo timeZone)
    {
        return TimeZoneInfo.ConvertTimeToUtc(dateTimeInJapanTimeZone, timeZone);
    }
}
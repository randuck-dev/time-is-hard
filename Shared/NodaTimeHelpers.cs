using System.Runtime.CompilerServices;
using NodaTime;
using NodaTime.TimeZones;

namespace Shared;

public static class NodaTimeHelpers
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ZonedDateTime ToZonedCopenhagen(DateTimeOffset dateTimeOffset)
    {
        var instant = Instant.FromDateTimeOffset(dateTimeOffset); 
        var timeZone = TzdbDateTimeZoneSource.Default.ForId("Europe/Copenhagen");
        return new ZonedDateTime(instant, timeZone);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ZonedDateTime ToZonedTokyo(DateTimeOffset dateTimeOffset)
    {
        var instant = Instant.FromDateTimeOffset(dateTimeOffset); 
        var timeZone = TzdbDateTimeZoneSource.Default.ForId("Asia/Tokyo");
        return new ZonedDateTime(instant, timeZone);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ZonedDateTime ToZonedWithTz(DateTimeOffset dateTimeOffset, DateTimeZone zone)
    {
        var instant = Instant.FromDateTimeOffset(dateTimeOffset); 
        return new ZonedDateTime(instant, zone);
    }
}
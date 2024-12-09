using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using NodaTime;
using NodaTime.TimeZones;
using Shared;

BenchmarkRunner.Run<Runner>();

[MemoryDiagnoser]
public class Runner
{
    private readonly DateTime _japan;
    private readonly DateTimeOffset _japanOffset;
    private readonly TimeZoneInfo _timeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
    private readonly DateTimeZone _tokyoTz;

    public Runner()
    {
        var dateOnly = new DateOnly(2024, 12, 31);
        var timeOnly = new TimeOnly(00, 00, 00);

        _japan = new DateTime(dateOnly, timeOnly, DateTimeKind.Unspecified);
        _japanOffset = new DateTimeOffset(dateOnly, timeOnly, TimeSpan.FromHours(9));
        _tokyoTz = TzdbDateTimeZoneSource.Default.ForId("Asia/Tokyo");
    }

    [Benchmark(Baseline = true)]
    public DateTimeOffset Normal()
    {
        return DateTimeOffsetHelpers.NaiveConversion(_japan);
    }

    [Benchmark]
    public DateTimeOffset Improved()
    {
        return DateTimeOffsetHelpers.InlineTimezoneLookupTokyo(_japan);
    }

    [Benchmark]
    public DateTimeOffset ImprovedEvenBetter()
    {
        return DateTimeOffsetHelpers.PassInTimezone(_japan, _timeZone);
    }
    
    [Benchmark]
    public DateTimeOffset ClassicalToUtc()
    {
        return DateTimeOffsetHelpers.ToUtc(_japanOffset);
    }

    [Benchmark]
    public ZonedDateTime ToZonedDateTime()
    {
        return NodaTimeHelpers.ToZonedTokyo(_japanOffset);
    }

    [Benchmark]
    public ZonedDateTime ToZonedDateTimeCachedTz()
    {
        return NodaTimeHelpers.ToZonedWithTz(_japanOffset, _tokyoTz);
    }
}
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run<Runner>();

[MemoryDiagnoser]
public class Runner
{
    private readonly DateTime _japan;
    private readonly TimeZoneInfo _timeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
    public Runner()
    {
        var dateOnly = new DateOnly(2024, 12, 31);
        var timeOnly = new TimeOnly(00, 00, 00);

        _japan = new DateTime(dateOnly, timeOnly, DateTimeKind.Unspecified);
    }

    [Benchmark(Baseline = true)]
    public DateTimeOffset Normal()
    {
        return new DateTimeOffset(_japan, TimeSpan.Zero);
    }

    [Benchmark]
    public DateTimeOffset Improved()
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
        return TimeZoneInfo.ConvertTimeToUtc(_japan, timeZone);
    }
    
    [Benchmark]
    public DateTimeOffset ImprovedEvenBetter()
    {
        return TimeZoneInfo.ConvertTimeToUtc(_japan, _timeZone);
    }
}
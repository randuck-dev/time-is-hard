﻿using NodaTime.TimeZones;
using Shared;

var jobMap = new Dictionary<string, Action>() {
  {"datetimeoffset", ExampleBuiltIn},
  {"nodatime", ExampleNodaTime},
  {"timezones", ExampleTimezones},
};

if (args.Length != 1)
{
    var jobNames = string.Join("\n", jobMap.Keys);
    System.Console.WriteLine($"Please provide exactly one of the following jobs:\n--- Jobs ---\n{jobNames}");
    Environment.Exit(-1);
}

var jobName = args[0].ToLowerInvariant();
if (jobName == "all")
{
    AllExamples();
    return;
}

jobMap[jobName]();

void ExampleBuiltIn()
{
    var basicDate = new DateTimeOffset(2024, 10, 27, 2, 0, 0, TimeSpan.FromHours(2));
    var basicDatePlusTwo = basicDate.AddHours(2);

    Console.WriteLine($"Time: {basicDate}");
    Console.WriteLine($"Time+2h: {basicDatePlusTwo}");
}

void ExampleNodaTime()
{
    var basicDate = new DateTimeOffset(2024, 10, 27, 2, 0, 0, TimeSpan.FromHours(2));
    var versionOfTimeZone = TzdbDateTimeZoneSource.Default.VersionId;
    var zonedDateTime = NodaTimeHelpers.ToZonedCopenhagen(basicDate);
    var zonedWithAddedHours = zonedDateTime.PlusHours(2);

    Console.WriteLine($"Version of Tzdb: {versionOfTimeZone}");
    Console.WriteLine($"Time: {zonedDateTime}");
    Console.WriteLine($"Time+2: {zonedWithAddedHours}");
}

void ExampleTimezones()
{
    var dateOnly = new DateOnly(2024, 12, 31);
    var timeOnly = new TimeOnly(00, 00, 00);
    var rawTokyoTime = new DateTime(dateOnly, timeOnly, DateTimeKind.Unspecified);
    var betterTokyoTime = new DateTimeOffset(dateOnly, timeOnly, TimeSpan.FromHours(9));
    var naive = DateTimeOffsetHelpers.NaiveConversionToTokyoTime(rawTokyoTime);
    var inLineTimezone = DateTimeOffsetHelpers.InlineTimezoneLookupTokyo(rawTokyoTime);
    var tokyoToUtcFromDateTimeOffset = DateTimeOffsetHelpers.ToUtc(betterTokyoTime);

    var tokyoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
    var passedTimezone = DateTimeOffsetHelpers.PassInTimezone(rawTokyoTime, tokyoTimeZone);

    Console.WriteLine($"Raw: {rawTokyoTime}");
    Console.WriteLine($"Naive: {naive}");
    Console.WriteLine($"Inline: {inLineTimezone}");
    Console.WriteLine($"Passed: {passedTimezone}");
    Console.WriteLine($"Better: {tokyoToUtcFromDateTimeOffset}");

}

void AllExamples()
{
    foreach (var pair in jobMap)
    {
        System.Console.WriteLine($"--- Job: {pair.Key} --- ");
        pair.Value();
        System.Console.WriteLine("----------\n");
    }
}
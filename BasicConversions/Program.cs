using Shared;

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


var dateOnly = new DateOnly(2024, 12, 31);
var timeOnly = new TimeOnly(00, 00, 00);
var dateTimeJapan = new DateTime(dateOnly, timeOnly, DateTimeKind.Unspecified);
var utc1 = ToUtc1(dateTimeJapan);
var utc2 = ToUtc2(dateTimeJapan);

var tokyoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
var utc3 = ToUtc3(dateTimeJapan, tokyoTimeZone);

Console.WriteLine(dateTimeJapan);
Console.WriteLine(utc1); 
Console.WriteLine(utc2);
Console.WriteLine(utc3);

DateTimeOffset ToUtc1(DateTime dateTime)
{
    return new DateTimeOffset(dateTime, TimeSpan.Zero);
}

DateTimeOffset ToUtc2(DateTime dateTimeInJapanTimeZone)
{
    var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
    return TimeZoneInfo.ConvertTimeToUtc(dateTimeInJapanTimeZone, timeZone);
}

DateTimeOffset ToUtc3(DateTime dateTimeInJapanTimeZone, TimeZoneInfo timeZone)
{
    return TimeZoneInfo.ConvertTimeToUtc(dateTimeInJapanTimeZone, timeZone);
}

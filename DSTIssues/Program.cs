using NodaTime.TimeZones;
using Shared;

var basicDate = new DateTimeOffset(2024, 10, 27, 2, 0, 0, TimeSpan.FromHours(2)); 
var basicDatePlusTwo = basicDate.AddHours(2);

var versionOfTimeZone = TzdbDateTimeZoneSource.Default.VersionId;
var zonedDateTime = NodaTimeHelpers.ToZonedCopenhagen(basicDate);
var zonedWithAddedHours = zonedDateTime.PlusHours(2);

Console.WriteLine("<Built-In>");
Console.WriteLine($"DateTimeOffset: {basicDate}");
Console.WriteLine($"DateTimeOffset+2h: {basicDatePlusTwo}");
Console.WriteLine("</Built-In>\n\n\n\n\n");

Console.WriteLine("<NodaTime>");
Console.WriteLine($"Version of Tzdb: {versionOfTimeZone}");
Console.WriteLine($"Zoned: {zonedDateTime}");
Console.WriteLine($"Zoned +2 Hours: {zonedWithAddedHours}");
Console.WriteLine("</ NodaTime>");

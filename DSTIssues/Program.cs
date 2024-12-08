using NodaTime;
using NodaTime.TimeZones;

var basicDate = new DateTimeOffset(2024, 10, 27, 2, 0, 0, TimeSpan.FromHours(2)); 

var basicDatePlusTwo = basicDate.AddHours(2);

var instant = Instant.FromDateTimeOffset(basicDate); 
var cphTimeZone = TzdbDateTimeZoneSource.Default.ForId("Europe/Copenhagen");
var zonedDateTime = new ZonedDateTime(instant, cphTimeZone);
var zonedWithAddedHours = zonedDateTime.PlusHours(2);


Console.WriteLine(basicDate);
Console.WriteLine(basicDatePlusTwo);
Console.WriteLine(instant);
Console.WriteLine(zonedDateTime);
Console.WriteLine(zonedWithAddedHours);
Console.WriteLine(zonedWithAddedHours.PlusHours(-2));

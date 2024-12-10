---
title: Time Is Hard 
sub_title: Navigating details of DST and other quirks of time
authors:
  - Raphael Neumann (rne@mft-energy.com / mail@raphaelneumann.dk)
theme:
  name: catppuccin-frappe
  override:
    footer:
      style: template
      left: "Raphael Neumann"
      center: "Time is Hard"
      right: "{current_slide} / {total_slides}"
---
Agenda
---


# Common Scenarios
# Designing our way out of this
# NodaTime


<!-- end_slide -->

Scenario (1) Basic Conversion
---

# Snippet Time

<!-- pause -->

```csharp
public static DateTimeOffset NaiveConversionToTokyoTime(DateTime dateTime)
{
    return new DateTimeOffset(dateTime.AddHours(-9), TimeSpan.Zero);
}
```

<!-- pause -->

# Any issues?

<!-- pause -->

<!-- incremental_lists: true -->

- Assuming that the DateTime is in Tokyo Time
- Hopefully DateTime.Kind is of type `Unspecified`
- What about timezone nuances?

<!-- incremental_lists: false -->

<!-- end_slide -->
Scenario (2) Timezone Lookup
---

# Snippet Time

```csharp
public static DateTimeOffset InlineTimezoneLookupTokyo(DateTime dateTime)
{
    var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
    return TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZone);
}
```

<!-- pause -->
# What about issues?
<!-- pause -->

<!-- incremental_lists: true -->

- Which version of the `Tokyo Standard Time`?
  - 2024a? 2024b? etc..

<!-- incremental_lists: false -->

<!-- end_slide -->
Scenario (3) DST Arithmetics P1
---

```csharp
var basicDate = new DateTimeOffset(2024, 10, 27, 2, 0, 0, TimeSpan.FromHours(2));
var basicDatePlusTwo = basicDate.AddHours(2);
```

<!-- pause  -->

```csharp
Console.WriteLine("<Built-In>");
Console.WriteLine($"DateTimeOffset: {basicDate}");
Console.WriteLine($"DateTimeOffset+2h: {basicDatePlusTwo}");
Console.WriteLine("</Built-In>\n\n\n\n\n");
```

<!-- end_slide -->
Scenario (4) DST Arithmetics P2
---

```csharp
var versionOfTimeZone = TzdbDateTimeZoneSource.Default.VersionId;
var zonedDateTime = NodaTimeHelpers.ToZonedCopenhagen(basicDate);
var zonedWithAddedHours = zonedDateTime.PlusHours(2);

Console.WriteLine("<NodaTime>");
Console.WriteLine($"Version of Tzdb: {versionOfTimeZone}");
Console.WriteLine($"Zoned: {zonedDateTime}");
Console.WriteLine($"Zoned +2 Hours: {zonedWithAddedHours}");
Console.WriteLine("</ NodaTime>");
```


<!-- end_slide -->
How to store the data?
---

```csharp
record Option1(DateTime UtcData);
```

- Original information is lost

```csharp
record Option2(DateTime UtcData, string IanaId, string TzdbVersion);
```

- Improved with TimeZoneInfo and which version of the Tzdb
- Still in UTC, but we can recalculate

```csharp
record Option3(DateTime Original, DateTimeOffset Derived, string IanaId, string TzdbVersion);
```

- Adheres to principle of "preserved data"
- Derived can be recalculated, without the original having to change

<!-- end_slide -->
Can we design our way out of these pitfalls?
---

<!-- end_slide -->
Resources for this presentation
---

# [`Storing UTC is not a silver bullet`](https://codeblog.jonskeet.uk/2019/03/27/storing-utc-is-not-a-silver-bullet/)

# [`When “UTC everywhere” isn’t enough - storing time zones in PostgreSQL and SQL Server`](https://www.roji.org/storing-timezones-in-the-db)



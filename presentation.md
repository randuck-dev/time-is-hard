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
## DateTimeOffset
##  NodaTime
# How to Store the data

# Past vs Future


<!-- end_slide -->

Scenario (1) Basic Conversion
---

# Snippet Time

<!-- pause -->

```csharp
public static DateTimeOffset FromTokyoTime(DateTime dateTime)
{
    return new DateTimeOffset(dateTime.AddHours(-9), TimeSpan.Zero);
}
```

<!-- pause -->

# Issues?

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
# Issues?
<!-- pause -->

<!-- incremental_lists: true -->

- Which version of the `Tokyo Standard Time`?
  - 2024a? 2024b? etc..

<!-- incremental_lists: false -->

<!-- end_slide -->
Scenario (3) DST Arithmetics P1 - BuiltIn
---

```csharp
var basicDate = new DateTimeOffset(2024, 10, 27, 2, 0, 0, TimeSpan.FromHours(2));
var basicDatePlusTwo = basicDate.AddHours(2);
```
<!-- pause  -->
# Issues?
<!-- pause  -->

```bash +exec
dotnet run --project Cli/Cli.csproj datetimeoffset
```

<!-- end_slide -->
Scenario (4) DST Arithmetics P1 - BuiltIn
---

```csharp
var basicDate = new DateTimeOffset(2024, 10, 27, 2, 0, 0, TimeSpan.FromHours(2));
var copenhagenTz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
var isDaylight = copenhagenTz.IsDaylightSavingTime(basicDate);
var addTwoHours = isDaylight ? basicDate.AddHours(2) : basicDate.AddHours(1);
var inZone = TimeZoneInfo.ConvertTime(addTwoHours, copenhagenTz);
```

<!-- pause  -->

```bash +exec
dotnet run --project Cli/Cli.csproj datetimeoffsetsolid
```

<!-- end_slide -->
Scenario (5) DST Arithmetics P2 - NodaTime
---

```csharp
var versionOfTimeZone = TzdbDateTimeZoneSource.Default.VersionId;
var zonedDateTime = NodaTimeHelpers.ToZonedCopenhagen(basicDate);
var zonedWithAddedHours = zonedDateTime.PlusHours(2);
```

<!-- pause  -->

```bash +exec
dotnet run --project Cli/Cli.csproj nodatime
```


<!-- end_slide -->
How do we store the data? Option 1
---

```json
{
  "name": "NiceReservation",
  "originalDate": "2026-01-10 12:00:00",
  "address": "Aarhus"
}
```

<!-- pause -->

```csharp
record Reservation(string Name, DateTime UtcDate);
```

<!-- pause -->

> Where did the original data go?
```json
{
  "name": "NiceReservation",
  "utcDate": "2026-01-10 11:00:00Z",
  "address": "Aarhus"
}
```



<!-- end_slide -->
How do we store the data? Option 2
---

```json
{
  "name": "NiceReservation",
  "originalDate": "2026-01-10 12:00:00",
  "address": "Aarhus"
}
```

<!-- pause -->

```csharp
record Reservation(DateTime UtcData, string IanaId, string TzdbVersion);
```
<!-- pause -->
<!-- incremental_lists: true -->
- Improved with TimeZoneInfo and which version of the Tzdb
- Still in UTC, but we can recalculate
<!-- incremental_lists: false -->

<!-- pause -->

<!-- column_layout: [2, 1, 2] -->
<!-- column: 0 --> 
```json
{
  "name": "NiceReservation",
  "utcDate": "2026-01-10 11:00:00Z",
  "ianaId": "Europe/Copenhagen",
  "tzdbVersion": "2024b",
  "address": "Aarhus"
}
```
<!-- pause -->
<!-- column: 1 -->

--> Now a change happens to the timezone definition

--> Use the old timezone rule to go back to local and then to the new one

<!-- column: 2 -->
```json
{
  "name": "NiceReservation",
  "utcDate": "2026-01-10 10:30:00Z",
  "ianaId": "Europe/Aarhus",
  "tzdbVersion": "2025a",
  "address": "Aarhus"
}
```

<!-- end_slide -->
How do we store the data? Option 3
---

```json
{
  "name": "NiceReservation",
  "originalDate": "2026-01-10 12:00:00",
  "address": "Aarhus"
}
```

<!-- pause -->
```csharp
record Reservation(DateTime Original, DateTimeOffset Derived, string IanaId, string TzdbVersion);
```
<!-- incremental_lists: true -->
- Adheres to principle of "preserved data"
- Derived can be recalculated, without the original having to change
<!-- incremental_lists: false -->

<!-- column_layout: [2, 1, 2] -->
<!-- column: 0 --> 
```json
{
  "name": "NiceReservation",
  "originalDate": "2026-01-10 12:00:00",
  "utcDate": "2026-01-10 11:00:00Z",
  "ianaId": "Europe/Copenhagen",
  "tzdbVersion": "2024b",
  "address": "Aarhus"
}
```
<!-- pause -->
<!-- column: 1 -->

--> Now a change happens to the timezone definition

--> Use the orignal data

<!-- column: 2 -->
```json
{
  "name": "NiceReservation",
  "originalDate": "2026-01-10 12:00:00",
  "utcDate": "2026-01-10 10:30:00Z",
  "ianaId": "Europe/Aarhus",
  "tzdbVersion": "2025a",
  "address": "Aarhus"
}
```

<!-- end_slide -->
Past vs Future
---

# Distant Past
- Saving UTC is safe 99% of the time

# Near Past
- You might be processing a datapoint from yesterday, but are not using the latest Tzdb that was released yesterday

# Future
- Save the original data and consider using `Option3` 

<!-- end_slide -->
Resources for this presentation
---

# [`Storing UTC is not a silver bullet`](https://codeblog.jonskeet.uk/2019/03/27/storing-utc-is-not-a-silver-bullet/)

# [`When “UTC everywhere” isn’t enough - storing time zones in PostgreSQL and SQL Server`](https://www.roji.org/storing-timezones-in-the-db)



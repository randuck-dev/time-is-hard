---
title: What time is it? 
sub_title: Navigating details of DST and other quirks of time
authors:
  - Raphael Neumann
theme:
  name: catppuccin-frappe
  override:
    footer:
      style: template
      left: "Raphael Neumann"
      center: "What time is it?"
      right: "{current_slide} / {total_slides}"
---
Agenda
---


# Some Timezones Details

# Common Scenarios
## DateTimeOffset
##  NodaTime
# How to Store the data

# Past vs Future

<!-- end_slide -->

Some Timezones Details
---

<!-- incremental_lists: true -->

- Decided by government for the country 
- Some timezones are 30 or 45 minutes
  - E.g. India, Nepal
- Offset ranges from UTC-12:00 -> UTC+14:00
- Some of them have a scary feature
  - DST

<!-- incremental_lists: false -->

<!-- end_slide -->

RFC3339 (ISO8601)
---

# Examples

<!-- pause -->

<!-- incremental_lists: true -->
- 2024-01-01T12:00:00Z
- 2024-01-01T12:00:00+00:00
- 2024-01-01T12:00:00-00:00
- 2024-01-01T12:00:00+02:00
- 2024-01-01T12:00:00+04:00
<!-- incremental_lists: false -->

<!-- pause -->

# What do they all lack?

<!-- pause -->

<!-- incremental_lists: true -->
- Timezone Information!
<!-- incremental_lists: false -->


<!-- end_slide -->

RFC9557 (ISO8601)
---
<!-- incremental_lists: true -->
- Approved as of April 2024
- Internet Extended Date/Time Format (IXDTF)
- 2022-07-08T02:14:07+02:00\[Europe/Paris\]
- 1996-12-19T16:39:57-08:00\[America/Los_Angeles\]\[u-ca=hebrew\]
<!-- incremental_lists: false -->

<!-- end_slide -->

What information does your computer have?
---

```bash +exec
cat /usr/share/zoneinfo/Europe/Copenhagen | tail -n 1
```
<!-- pause -->

-> Translates to
- First part: CET to CEST at the last sunday in March at 3 in the morning
- Second part: Go back to CET from CEST in the last sunday of october at 3 am local 


<!-- end_slide -->
Scenario (1) Basic Conversion
---

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
  - 2018a? 2024a? 2024b? etc..

<!-- incremental_lists: false -->

<!-- end_slide -->
Scenario (3) DST Arithmetics P1 - BuiltIn
---


```csharp
var copenhagenDate = new DateTimeOffset(2024, 10, 27, 2, 0, 0, TimeSpan.FromHours(2));
var copenhagenDatePlusTwo = copenhagenDate.AddHours(2);
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
var isDaylight = copenhagenTz.IsDaylightSavingTime(copenhagenDate);
var addTwoHours = isDaylight ? copenhagenDate.AddHours(2) : copenhagenDate.AddHours(1);
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
var instant = Instant.FromDateTimeOffset(copenhagenDate); 
var zonedDateTime = new ZonedDateTime(instant, timeZone);
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
record Reservation(string Name, DateTime UtcDate, string Address);
```

<!-- pause -->

> Where did the original data go?
```json
{
  "name": "NiceReservation",
  "utcDate": "2026-01-10T11:00:00Z",
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
record Reservation(DateTime UtcDate, string Address, string IanaId, string TzdbVersion);
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
  "utcDate": "2026-01-10T11:00:00Z",
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
  "utcDate": "2026-01-10T10:30:00Z",
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
record Reservation(DateTime OriginalDate, DateTimeOffset UtcDate, string Address, string IanaId, string TzdbVersion);
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
  "utcDate": "2026-01-10T11:00:00Z",
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
  "utcDate": "2026-01-10T10:30:00Z",
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

<!-- pause -->

# Near Past
- You might be processing a datapoint from yesterday, but are not using the latest Tzdb that was released yesterday

<!-- pause -->
# Future
- Save the original data and consider using `Option3` 

```csharp
record Reservation(DateTime OriginalDate, DateTimeOffset UtcDate, string Address, string IanaId, string TzdbVersion);
```
<!-- end_slide -->

Final Notes
---
<!-- incremental_lists: true -->
- What about syncing time?
  - NTP
  - Atomic Clocks
- How do other languages handle this?
- Did I cover everything around timezones?
<!-- incremental_lists: false -->

<!-- end_slide -->


Resources for this presentation
---

# [`Storing UTC is not a silver bullet`](https://codeblog.jonskeet.uk/2019/03/27/storing-utc-is-not-a-silver-bullet/)

# [`Australia/Lord_Howe is the weirdest timezone`](https://ssoready.com/blog/engineering/truths-programmers-timezones/)

# [`When “UTC everywhere” isn’t enough - storing time zones in PostgreSQL and SQL Server`](https://www.roji.org/storing-timezones-in-the-db)
# [`RFC3339`](https://datatracker.ietf.org/doc/html/rfc3339)

# [`RFC9557`](https://datatracker.ietf.org/doc/html/rfc9557)

# [`RFC8536`](https://datatracker.ietf.org/doc/html/rfc8536)



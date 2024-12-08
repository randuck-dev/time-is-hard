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


# Common pitfalls
# Designing our way out of this
# NodaTime


<!-- end_slide -->

Let's talk everyday average code
---

# Something about basic mistakes we can make

```csharp
public DateTimeOffset FromAustraliaToUtc(DateTime dateTime) {
    return new DateTimeOffset(datetime, TimeSpan.FromHours(0));
}
```

<!-- pause -->

# Which problems do you see in this snippet?

<!-- pause -->

<!-- incremental_lists: true -->
- Assuming that the datetime is in Australia Time
- Hopefully DateTime.Kind is Unspecified



<!-- incremental_lists: false -->


<!-- end_slide -->

Can we design our way out of these pitfalls?
---


using System.Collections.Concurrent;
using System.Globalization;

namespace Botticelli.Framework.Controls.Layouts.Inlines;

public static class CalendarFactory
{
    private static ConcurrentDictionary<(int month, int year, string culture), InlineCalendar> Cache => new();

    public static InlineCalendar Get(DateTime dt, string culture)
    {
        var key = (dt.Month, dt.Year, culture);

        if (Cache.TryGetValue(key, out var calendar))
            return calendar;

        var cultureInfo = new CultureInfo(culture);

        calendar = new InlineCalendar(dt, cultureInfo);

        Cache[key] = calendar;

        return calendar;
    }

    public static InlineCalendar GetMonthsForward(string culture, int months = 1)
        => GetMonthsForward(DateTime.Today, culture, months);

    public static InlineCalendar GetMonthsForward(DateTime dt, string culture, int months = 1)
        => Get(dt.AddMonths(months), culture);

    public static InlineCalendar GetMonthsBackward(string culture, int months = 1)
        => GetMonthsForward(DateTime.Today, culture, -months);

    public static InlineCalendar GetMonthsBackward(DateTime dt, string culture, int months = 1)
        => GetMonthsForward(dt, culture, -months);
}
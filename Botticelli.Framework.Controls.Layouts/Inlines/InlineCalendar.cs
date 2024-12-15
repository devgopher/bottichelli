using System.Globalization;
using Botticelli.Framework.Controls.BasicControls;

namespace Botticelli.Framework.Controls.Layouts.Inlines;

public class InlineCalendar : ILayout
{
    private static readonly DayOfWeek[] Days =
    [
        DayOfWeek.Sunday,
        DayOfWeek.Monday,
        DayOfWeek.Tuesday,
        DayOfWeek.Wednesday,
        DayOfWeek.Thursday,
        DayOfWeek.Friday,
        DayOfWeek.Saturday
    ];

    public InlineCalendar(DateTime dt, CultureInfo cultureInfo)
    {
        Rows = new List<Row>(5);
        Init(dt, cultureInfo);
    }

    /// <summary>
    ///     Today checkmark image  path
    /// </summary>
    public string TodayMark { get; set; }

    public void AddRow(Row row) => Rows.Add(row);

    public IList<Row>? Rows { get; }

    private void Init(DateTime dt, CultureInfo cultureInfo)
    {
        // Displays a year in a header
        var yearNameRow = new Row();
        yearNameRow.Items.Add(new Item
        {
            Control = new Button
            {
                Content = "<<", Params = new Dictionary<string, string>
                {
                    { "CallbackData", $"/YearBackward {dt:dd/MM/yyyy}" }
                }
            }
        });
        yearNameRow.Items.Add(new Item { Control = new Button { Content = dt.ToString("yyyy") } });
        yearNameRow.Items.Add(new Item
        {
            Control = new Button
            {
                Content = ">>", Params = new Dictionary<string, string>
                {
                    { "CallbackData", $"/YearForward {dt:dd/MM/yyyy}" }
                }
            }
        });
        Rows.Add(yearNameRow);

        // Displays a month name in a header
        var monthName = cultureInfo.DateTimeFormat.MonthNames[dt.Month - 1];
        var monthNameRow = new Row();
        monthNameRow.Items.Add(new Item
        {
            Control = new Button
            {
                Content = "<<", Params = new Dictionary<string, string>
                {
                    { "CallbackData", $"/MonthBackward {dt:dd/MM/yyyy}" }
                }
            }
        });
        monthNameRow.Items.Add(new Item { Control = new Button { Content = monthName } });
        monthNameRow.Items.Add(new Item
        {
            Control = new Button
            {
                Content = ">>", Params = new Dictionary<string, string>
                {
                    { "CallbackData", $"/MonthForward {dt:dd/MM/yyyy}" }
                }
            }
        });

        Rows.Add(monthNameRow);

        // Displays a weekday names in a header
        var sortedDays = new DayOfWeek[Days.Length];
        var fdw = (int)cultureInfo.DateTimeFormat.FirstDayOfWeek;
        for (var i = 0; i < Days.Length; ++i) sortedDays[i] = Days[(fdw + i) % Days.Length];

        var weekDaysRow = new Row();
        weekDaysRow.Items.AddRange(sortedDays.Select(sd => new Item
            { Control = new Button { Content = sd.ToString("G") } }));
        Rows.Add(weekDaysRow);

        // Displays dates
        var days = CultureInfo.InvariantCulture.Calendar.GetDaysInMonth(dt.Year, dt.Month);

        var rows = new Row?[6];

        for (var day = 1; day <= days; ++day)
        {
            var dayOfWeek = (int)CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(new DateTime(dt.Year, dt.Month, 1));
            var offset = dayOfWeek - 1;
            var rowNum = (day + offset) / Days.Length;

            rows[rowNum] ??= new Row();

            PreloadItems(rows, rowNum);
            var buttonDt = new DateTime(day: day, month: dt.Month, year: dt.Year);

            rows[rowNum]!.Items[(day + offset) % Days.Length] = new Item
            {
                Control = new Button
                {
                    Content = $"{GetImage(buttonDt)}{day}",
                    Image = GetImage(buttonDt),
                    CallbackData = $"/DateChosen {buttonDt:dd/MM/yyyy}"
                }
            };

            var weekDayOffset = (day + offset) % Days.Length;
            rows[rowNum]!.Items[weekDayOffset].Control!.CallbackData
                = $"/DateChosen {new DateTime(day: day, month: dt.Month, year: dt.Year):dd/MM/yyyy}";
        }

        foreach (var row in rows)
            Rows.Add(row!);
    }

    private string GetImage(DateTime buttonDt) => DateTime.Today == buttonDt.Date
        ? !string.IsNullOrWhiteSpace(TodayMark) ? TodayMark : "✓"
        : string.Empty;

    private static void PreloadItems(Row?[] rows, int rowNum)
    {
        var cnt = rows[rowNum]!.Items.Count;

        if (cnt >= Days.Length) return;

        for (var i = cnt; i < Days.Length; ++i)
            rows[rowNum]!.Items.Add(new Item
            {
                Control = new Button
                {
                    Content = "-"
                }
            });
    }
}
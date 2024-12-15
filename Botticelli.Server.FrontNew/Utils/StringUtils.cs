using System.Globalization;

namespace Botticelli.Server.FrontNew.Utils;

public static class StringUtils
{
    public static DateTime? ToDateTime(this string dt, string format, CultureInfo culture)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dt)) return default;

            return DateTime.ParseExact(dt, format, culture);
        }
        catch (Exception ex)
        {
            return default;
        }
    }
}
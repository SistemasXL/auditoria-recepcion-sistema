namespace AuditoriaRecepcion.Helpers.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToShortDateString(this DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }

        public static string ToFullDateTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm:ss");
        }

        public static DateTime StartOfDay(this DateTime date)
        {
            return date.Date;
        }

        public static DateTime EndOfDay(this DateTime date)
        {
            return date.Date.AddDays(1).AddTicks(-1);
        }

        public static DateTime StartOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime EndOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddTicks(-1);
        }

        public static int GetWeekOfYear(this DateTime date)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            return culture.Calendar.GetWeekOfYear(
                date,
                System.Globalization.CalendarWeekRule.FirstDay,
                DayOfWeek.Monday);
        }

        public static string ToRelativeTimeString(this DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan.TotalSeconds < 60)
                return "hace unos segundos";
            else if (timeSpan.TotalMinutes < 60)
                return $"hace {(int)timeSpan.TotalMinutes} minuto(s)";
            else if (timeSpan.TotalHours < 24)
                return $"hace {(int)timeSpan.TotalHours} hora(s)";
            else if (timeSpan.TotalDays < 7)
                return $"hace {(int)timeSpan.TotalDays} día(s)";
            else if (timeSpan.TotalDays < 30)
                return $"hace {(int)(timeSpan.TotalDays / 7)} semana(s)";
            else if (timeSpan.TotalDays < 365)
                return $"hace {(int)(timeSpan.TotalDays / 30)} mes(es)";
            else
                return $"hace {(int)(timeSpan.TotalDays / 365)} año(s)";
        }
    }
}
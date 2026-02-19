using System.Globalization;

namespace Dewit.Core.Utils
{
    public static class DateParser
    {
        private static readonly string[] DayNames =
            ["monday", "tuesday", "wednesday", "thursday", "friday", "saturday", "sunday"];

        public static DateTime Parse(string input)
        {
            var normalized = input.Trim().ToLowerInvariant();

            DateTime result = normalized switch
            {
                "today"     => DateTime.Today,
                "yesterday" => DateTime.Today.AddDays(-1),
                _           => TryParseNaturalOrFormatted(normalized)
            };

            if (result.Date > DateTime.Today)
                throw new ArgumentException($"Date '{input}' is in the future. Only past or today dates are allowed.");

            return result.Date; // strip time component
        }

        private static DateTime TryParseNaturalOrFormatted(string input)
        {
            // "last <weekday>"
            if (input.StartsWith("last "))
            {
                var dayPart = input[5..].Trim();
                var idx = Array.IndexOf(DayNames, dayPart);
                if (idx >= 0)
                    return GetLastWeekday((DayOfWeek)((idx + 1) % 7));
            }

            // "YYYY-MM-DD"
            if (DateTime.TryParseExact(input, "yyyy-MM-dd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var full))
                return full;

            // "MM-DD" → assume current year
            if (DateTime.TryParseExact($"{DateTime.Today.Year}-{input}", "yyyy-MM-dd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var monthDay))
                return monthDay;

            throw new ArgumentException(
                $"Cannot parse date '{input}'. Accepted formats: today, yesterday, last monday, YYYY-MM-DD, MM-DD.");
        }

        private static DateTime GetLastWeekday(DayOfWeek target)
        {
            var today = DateTime.Today;
            var daysBack = ((int)today.DayOfWeek - (int)target + 7) % 7;
            if (daysBack == 0) daysBack = 7; // "last monday" when today is Monday = 7 days ago
            return today.AddDays(-daysBack);
        }
    }
}

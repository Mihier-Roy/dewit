using System;
using System.CommandLine;
using System.Globalization;
using Dewit.CLI.Utils;
using Dewit.Core.Interfaces;
using Serilog;

namespace Dewit.CLI.Commands.Mood
{
    public class ViewMoodCommand : Command
    {
        private readonly IMoodService _moodService;
        private readonly Option<string> _durationOpt;
        private readonly Option<string?> _periodOpt;

        public ViewMoodCommand(IMoodService moodService)
            : base("view", "Display your mood calendar.")
        {
            _moodService = moodService;

            _durationOpt = new Option<string>("--duration")
            {
                Description = "Calendar range: week (default), month, quarter, year.",
                DefaultValueFactory = _ => "week",
            };
            _durationOpt.AcceptOnlyFromAmong("week", "month", "quarter", "year");

            _periodOpt = new Option<string?>("--period")
            {
                Description =
                    "Period to display. "
                    + "month: YYYY-MM (e.g. 2026-02). "
                    + "quarter: YYYY-Q# (e.g. 2026-Q1). "
                    + "year: YYYY (e.g. 2026). "
                    + "Defaults to the current period.",
            };

            this.Options.Add(_durationOpt);
            this.Options.Add(_periodOpt);

            this.SetAction(parseResult =>
            {
                var duration = parseResult.GetValue(_durationOpt)!;
                var period = parseResult.GetValue(_periodOpt);
                Run(duration, period);
            });
        }

        private void Run(string duration, string? period)
        {
            try
            {
                switch (duration)
                {
                    case "week":
                        RenderWeek();
                        break;
                    case "month":
                        RenderMonth(period);
                        break;
                    case "quarter":
                        RenderQuarter(period);
                        break;
                    case "year":
                        RenderYear(period);
                        break;
                }
            }
            catch (ArgumentException ex)
            {
                Output.WriteError(ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to render mood calendar");
                Output.WriteError("Failed to display mood calendar. Please try again.");
            }
        }

        private void RenderWeek()
        {
            var monday = GetMonday(DateTime.Today);
            var sunday = monday.AddDays(6);
            var entries = _moodService.GetEntriesInRange(monday, sunday);
            MoodCalendar.RenderWeek(DateTime.Today, entries);
        }

        private void RenderMonth(string? period)
        {
            int year,
                month;
            if (period == null)
            {
                year = DateTime.Today.Year;
                month = DateTime.Today.Month;
            }
            else if (
                DateTime.TryParseExact(
                    period,
                    "yyyy-MM",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var parsed
                )
            )
            {
                year = parsed.Year;
                month = parsed.Month;
            }
            else
            {
                throw new ArgumentException(
                    $"Invalid month period '{period}'. Use YYYY-MM format, e.g. 2026-02."
                );
            }

            var from = new DateTime(year, month, 1);
            var to = from.AddMonths(1).AddDays(-1);
            var entries = _moodService.GetEntriesInRange(from, to);
            MoodCalendar.RenderMonth(year, month, entries);
        }

        private void RenderQuarter(string? period)
        {
            int year,
                quarter;
            if (period == null)
            {
                year = DateTime.Today.Year;
                quarter = (DateTime.Today.Month - 1) / 3 + 1;
            }
            else
            {
                // Expects "YYYY-Q#"
                var parts = period.Split('-');
                if (
                    parts.Length != 2
                    || !int.TryParse(parts[0], out year)
                    || parts[1].Length != 2
                    || parts[1][0] != 'Q'
                    || !int.TryParse(parts[1][1..], out quarter)
                    || quarter < 1
                    || quarter > 4
                )
                {
                    throw new ArgumentException(
                        $"Invalid quarter period '{period}'. Use YYYY-Q# format, e.g. 2026-Q1."
                    );
                }
            }

            var startMonth = (quarter - 1) * 3 + 1;
            var from = new DateTime(year, startMonth, 1);
            var to = from.AddMonths(3).AddDays(-1);
            var entries = _moodService.GetEntriesInRange(from, to);
            MoodCalendar.RenderQuarter(year, quarter, entries);
        }

        private void RenderYear(string? period)
        {
            int year;
            if (period == null)
            {
                year = DateTime.Today.Year;
            }
            else if (!int.TryParse(period, out year))
            {
                throw new ArgumentException(
                    $"Invalid year period '{period}'. Use YYYY format, e.g. 2026."
                );
            }

            var from = new DateTime(year, 1, 1);
            var to = new DateTime(year, 12, 31);
            var entries = _moodService.GetEntriesInRange(from, to);
            MoodCalendar.RenderYear(year, entries);
        }

        private static DateTime GetMonday(DateTime date)
        {
            var diff = ((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            return date.AddDays(-diff).Date;
        }
    }
}

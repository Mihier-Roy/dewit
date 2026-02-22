using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Globalization;
using System.IO;
using System.Linq;
using Dewit.CLI.Utils;
using Dewit.Core.Interfaces;
using Spectre.Console;

namespace Dewit.CLI.Commands.Journal
{
    public class ViewJournalCommand : Command
    {
        private readonly IMoodService _moodService;
        private readonly IJournalService _journalService;
        private readonly Option<string> _durationOpt;
        private readonly Option<string?> _periodOpt;

        public ViewJournalCommand(IMoodService moodService, IJournalService journalService)
            : base("view", "Display your mood calendar with journal entry indicators.")
        {
            _moodService = moodService;
            _journalService = journalService;

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

            Options.Add(_durationOpt);
            Options.Add(_periodOpt);

            SetAction(parseResult =>
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
                Output.WriteVerbose(ex, "Failed to render journal calendar");
                Output.WriteError("Failed to display calendar. Please try again.");
            }
        }

        private void RenderWeek()
        {
            var monday = GetMonday(DateTime.Today);
            var sunday = monday.AddDays(6);
            var moodEntries = _moodService.GetEntriesInRange(monday, sunday).ToList();
            var journalDates = GetJournalDates(monday, sunday);
            RunWithToggle(
                (show, jDates) =>
                    MoodCalendar.RenderWeek(DateTime.Today, moodEntries, show, jDates),
                journalDates
            );
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
            var moodEntries = _moodService.GetEntriesInRange(from, to).ToList();
            var journalDates = GetJournalDates(from, to);
            RunWithToggle(
                (show, jDates) => MoodCalendar.RenderMonth(year, month, moodEntries, show, jDates),
                journalDates
            );
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
            var moodEntries = _moodService.GetEntriesInRange(from, to).ToList();
            var journalDates = GetJournalDates(from, to);
            RunWithToggle(
                (show, jDates) =>
                    MoodCalendar.RenderQuarter(year, quarter, moodEntries, show, jDates),
                journalDates
            );
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
            var moodEntries = _moodService.GetEntriesInRange(from, to).ToList();
            var journalDates = GetJournalDates(from, to);
            RunWithToggle(
                (show, jDates) => MoodCalendar.RenderYear(year, moodEntries, show, jDates),
                journalDates
            );
        }

        private HashSet<DateTime> GetJournalDates(DateTime from, DateTime to) =>
            _journalService.GetEntriesInRange(from, to).Select(e => e.Date).ToHashSet();

        private void RunWithToggle(
            Action<bool, HashSet<DateTime>?> render,
            HashSet<DateTime> journalDates
        )
        {
            var showDescriptors = false;
            while (true)
            {
                AnsiConsole.Clear();
                render(showDescriptors, journalDates.Count > 0 ? journalDates : null);
                var key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.D)
                {
                    showDescriptors = !showDescriptors;
                }
                else if (key.Key == ConsoleKey.J && journalDates.Count > 0)
                {
                    OpenJournalEntry(journalDates);
                }
                else
                {
                    break;
                }
            }
        }

        private void OpenJournalEntry(HashSet<DateTime> journalDates)
        {
            AnsiConsole.Clear();
            var sorted = journalDates.OrderByDescending(d => d).ToList();

            var prompt = new SelectionPrompt<DateTime>()
                .Title("Select a journal entry to open:")
                .UseConverter(d => d.ToString("ddd, MMM d, yyyy"))
                .AddChoices(sorted);

            var selected = AnsiConsole.Prompt(prompt);
            var entry = _journalService.GetEntryForDate(selected);

            if (entry == null || !File.Exists(entry.FilePath))
            {
                Output.WriteText($"[yellow]Journal file not found for {selected:yyyy-MM-dd}.[/]");
                Console.ReadKey(intercept: true);
                return;
            }

            EditorHelper.Open(entry.FilePath);
            _journalService.TouchUpdatedAt(selected);
        }

        private static DateTime GetMonday(DateTime date)
        {
            var diff = ((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            return date.AddDays(-diff).Date;
        }
    }
}

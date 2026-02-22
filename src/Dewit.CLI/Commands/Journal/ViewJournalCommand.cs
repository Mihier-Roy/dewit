using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Globalization;
using System.IO;
using System.Linq;
using Dewit.CLI.Utils;
using Dewit.Core.Entities;
using Dewit.Core.Enums;
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
            RunInteractive(
                moodEntries,
                (selectedIndex, jDates) =>
                    MoodCalendar.RenderWeek(DateTime.Today, moodEntries, selectedIndex, jDates),
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
            RunInteractive(
                moodEntries,
                (selectedIndex, jDates) =>
                    MoodCalendar.RenderMonth(year, month, moodEntries, selectedIndex, jDates),
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
            RunInteractive(
                moodEntries,
                (selectedIndex, jDates) =>
                    MoodCalendar.RenderQuarter(year, quarter, moodEntries, selectedIndex, jDates),
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
            RunInteractive(
                moodEntries,
                (selectedIndex, jDates) =>
                    MoodCalendar.RenderYear(year, moodEntries, selectedIndex, jDates),
                journalDates
            );
        }

        private HashSet<DateTime> GetJournalDates(DateTime from, DateTime to) =>
            _journalService.GetEntriesInRange(from, to).Select(e => e.Date).ToHashSet();

        private void RunInteractive(
            List<MoodEntry> moodEntries,
            Action<int, HashSet<DateTime>?> render,
            HashSet<DateTime> journalDates
        )
        {
            var jDates = journalDates.Count > 0 ? journalDates : null;
            var selectedIndex = -1;

            while (true)
            {
                AnsiConsole.Clear();
                render(selectedIndex, jDates);
                var key = Console.ReadKey(intercept: true);

                if (selectedIndex < 0)
                {
                    // Calendar mode: d enters detail nav, anything else exits
                    if (key.Key == ConsoleKey.D)
                    {
                        var entries = MoodCalendar.GetDetailEntries(moodEntries, jDates);
                        if (entries.Count > 0)
                            selectedIndex = 0;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    // Detail nav mode
                    var detailEntries = MoodCalendar.GetDetailEntries(moodEntries, jDates);

                    switch (key.Key)
                    {
                        case ConsoleKey.UpArrow:
                            selectedIndex = Math.Max(0, selectedIndex - 1);
                            break;

                        case ConsoleKey.DownArrow:
                            selectedIndex = Math.Min(
                                detailEntries.Count - 1,
                                selectedIndex + 1
                            );
                            break;

                        case ConsoleKey.Enter when detailEntries.Count > 0:
                            {
                                var moodEntry = detailEntries[selectedIndex];
                                var hasJournal = jDates?.Contains(moodEntry.Date) == true;

                                if (hasJournal)
                                {
                                    var journalEntry = _journalService.GetEntryForDate(moodEntry.Date);
                                    if (journalEntry != null && File.Exists(journalEntry.FilePath))
                                    {
                                        RunJournalView(journalEntry, moodEntry);
                                    }
                                    else
                                    {
                                        AnsiConsole.Clear();
                                        AnsiConsole.MarkupLine(
                                            $"[yellow]Journal file not found for {moodEntry.Date:yyyy-MM-dd}.[/]"
                                        );
                                        AnsiConsole.MarkupLine("[grey]Press any key to continue.[/]");
                                        Console.ReadKey(intercept: true);
                                    }
                                }
                                break;
                            }

                        case ConsoleKey.B:
                        case ConsoleKey.Backspace:
                            selectedIndex = -1;
                            break;

                        case ConsoleKey.Escape:
                            return;
                    }
                }
            }
        }

        private void RunJournalView(JournalEntry journalEntry, MoodEntry moodEntry)
        {
            while (true)
            {
                AnsiConsole.Clear();
                RenderJournalContent(journalEntry, moodEntry);
                var key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.E)
                {
                    EditorHelper.Open(journalEntry.FilePath);
                    _journalService.TouchUpdatedAt(journalEntry.Date);
                }
                else
                {
                    return; // any other key goes back
                }
            }
        }

        private static void RenderJournalContent(JournalEntry journalEntry, MoodEntry moodEntry)
        {
            if (Enum.TryParse<Mood>(moodEntry.Mood, out var mood))
            {
                var color = mood.ToSpectreColor();
                AnsiConsole.MarkupLine(
                    $"\n[bold]{journalEntry.Date:ddd, MMM d, yyyy}[/]  [{color}]{mood.ToDisplayName()}[/]"
                );

                if (!string.IsNullOrWhiteSpace(moodEntry.Descriptors))
                {
                    var descriptors = string.Join(
                        ", ",
                        moodEntry.Descriptors.Split(
                            ',',
                            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
                        )
                    );
                    AnsiConsole.MarkupLine($"[aqua]{descriptors}[/]");
                }
            }
            else
            {
                AnsiConsole.MarkupLine($"\n[bold]{journalEntry.Date:ddd, MMM d, yyyy}[/]");
            }

            AnsiConsole.WriteLine();

            if (File.Exists(journalEntry.FilePath))
            {
                var content = File.ReadAllText(journalEntry.FilePath).Trim();

                if (string.IsNullOrWhiteSpace(content))
                {
                    AnsiConsole.MarkupLine("[grey](no journal content)[/]");
                }
                else
                {
                    var termWidth = AnsiConsole.Console.Profile.Width;
                    var maxWidth = Math.Min(80, termWidth);
                    var hPad = Math.Max(0, (termWidth - maxWidth) / 2);

                    AnsiConsole.Write(
                        new Padder(
                            new Panel(new Text(content)).RoundedBorder().Expand(),
                            new Padding(hPad, 0, hPad, 0)
                        )
                    );
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[grey](journal file not found)[/]");
            }

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[grey]  e: edit   any other key: back[/]");
        }

        private static DateTime GetMonday(DateTime date)
        {
            var diff = ((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            return date.AddDays(-diff).Date;
        }
    }
}
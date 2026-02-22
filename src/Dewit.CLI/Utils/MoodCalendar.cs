using System;
using System.Collections.Generic;
using System.Linq;
using Dewit.Core.Entities;
using Dewit.Core.Enums;
using Spectre.Console;

namespace Dewit.CLI.Utils
{
    public static class MoodCalendar
    {
        private const string EmptyCell = "[grey]░░░░░░[/]";
        private const string BlockCell = "██████"; // colored by mood
        private static readonly string[] DayHeaders =
        [
            "Mon",
            "Tue",
            "Wed",
            "Thu",
            "Fri",
            "Sat",
            "Sun",
        ];

        /// <summary>Returns entries that have descriptors or journal entries, ordered by date.</summary>
        public static List<MoodEntry> GetDetailEntries(
            IEnumerable<MoodEntry> entries,
            HashSet<DateTime>? journalDates = null
        ) =>
            entries
                .Where(e =>
                    !string.IsNullOrWhiteSpace(e.Descriptors)
                    || (journalDates?.Contains(e.Date) == true)
                )
                .OrderBy(e => e.Date)
                .ToList();

        /// <summary>Renders a week grid (Mon–Sun of the given week containing 'weekDay').</summary>
        public static void RenderWeek(
            DateTime weekDay,
            IEnumerable<MoodEntry> entries,
            int selectedDescriptorIndex = -1,
            HashSet<DateTime>? journalDates = null
        )
        {
            var entryList = entries.ToList();
            var monday = GetMonday(weekDay);
            var days = Enumerable.Range(0, 7).Select(i => monday.AddDays(i)).ToList();
            var entryMap = entryList.ToDictionary(e => e.Date, e => e);

            AnsiConsole.MarkupLine($"\n[bold]Mood Calendar — Week of {monday:MMM d, yyyy}[/]\n");
            RenderWeekGrid(days, entryMap, journalDates);
            RenderLegend();

            if (selectedDescriptorIndex >= 0)
            {
                RenderDescriptorDetails(entryList, journalDates, selectedDescriptorIndex);
                RenderHint(inNavMode: true);
            }
            else
            {
                RenderHint(inNavMode: false);
            }
        }

        /// <summary>Renders a monthly grid (all weeks covering the month).</summary>
        public static void RenderMonth(
            int year,
            int month,
            IEnumerable<MoodEntry> entries,
            int selectedDescriptorIndex = -1,
            HashSet<DateTime>? journalDates = null
        )
        {
            var entryList = entries.ToList();
            var firstDay = new DateTime(year, month, 1);
            var lastDay = firstDay.AddMonths(1).AddDays(-1);
            var entryMap = entryList.ToDictionary(e => e.Date, e => e);

            AnsiConsole.MarkupLine($"\n[bold]Mood Calendar — {firstDay:MMMM yyyy}[/]\n");
            RenderMonthGrid(firstDay, lastDay, entryMap, journalDates);
            RenderLegend();

            if (selectedDescriptorIndex >= 0)
            {
                RenderDescriptorDetails(entryList, journalDates, selectedDescriptorIndex);
                RenderHint(inNavMode: true);
            }
            else
            {
                RenderHint(inNavMode: false);
            }
        }

        /// <summary>Renders a quarter grid (3 months side by side).</summary>
        public static void RenderQuarter(
            int year,
            int quarter,
            IEnumerable<MoodEntry> entries,
            int selectedDescriptorIndex = -1,
            HashSet<DateTime>? journalDates = null
        )
        {
            var entryList = entries.ToList();
            var startMonth = (quarter - 1) * 3 + 1;
            var entryMap = entryList.ToDictionary(e => e.Date, e => e);

            AnsiConsole.MarkupLine($"\n[bold]Mood Calendar — Q{quarter} {year}[/]\n");

            for (int m = startMonth; m < startMonth + 3; m++)
            {
                var firstDay = new DateTime(year, m, 1);
                var lastDay = firstDay.AddMonths(1).AddDays(-1);
                AnsiConsole.MarkupLine($"[bold]{firstDay:MMMM}[/]");
                RenderMonthGrid(firstDay, lastDay, entryMap, journalDates);
                AnsiConsole.WriteLine();
            }

            RenderLegend();

            if (selectedDescriptorIndex >= 0)
            {
                RenderDescriptorDetails(entryList, journalDates, selectedDescriptorIndex);
                RenderHint(inNavMode: true);
            }
            else
            {
                RenderHint(inNavMode: false);
            }
        }

        /// <summary>Renders a year grid (12 months).</summary>
        public static void RenderYear(
            int year,
            IEnumerable<MoodEntry> entries,
            int selectedDescriptorIndex = -1,
            HashSet<DateTime>? journalDates = null
        )
        {
            var entryList = entries.ToList();
            var entryMap = entryList.ToDictionary(e => e.Date, e => e);

            AnsiConsole.MarkupLine($"\n[bold]Mood Calendar — {year}[/]\n");

            for (int m = 1; m <= 12; m++)
            {
                var firstDay = new DateTime(year, m, 1);
                var lastDay = firstDay.AddMonths(1).AddDays(-1);
                AnsiConsole.MarkupLine($"[bold]{firstDay:MMMM}[/]");
                RenderMonthGrid(firstDay, lastDay, entryMap, journalDates);
                AnsiConsole.WriteLine();
            }

            RenderLegend();

            if (selectedDescriptorIndex >= 0)
            {
                RenderDescriptorDetails(entryList, journalDates, selectedDescriptorIndex);
                RenderHint(inNavMode: true);
            }
            else
            {
                RenderHint(inNavMode: false);
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private static void RenderWeekGrid(
            List<DateTime> days,
            Dictionary<DateTime, MoodEntry> entryMap,
            HashSet<DateTime>? journalDates
        )
        {
            var table = new Table().NoBorder().HideHeaders();
            foreach (var _ in days)
                table.AddColumn(new TableColumn("").Centered());

            // Row 1: day names + date numbers
            table.AddRow(days.Select(d => $"[bold]{d:ddd}[/]\n[grey]{d.Day, 2}[/]").ToArray());

            // Row 2: colored blocks
            table.AddRow(days.Select(d => DayCell(d, entryMap, journalDates)).ToArray());

            AnsiConsole.Write(table);
        }

        private static void RenderMonthGrid(
            DateTime firstDay,
            DateTime lastDay,
            Dictionary<DateTime, MoodEntry> entryMap,
            HashSet<DateTime>? journalDates
        )
        {
            // Find the Monday on or before the first day of the month
            var startMonday = GetMonday(firstDay);

            var table = new Table().NoBorder().HideHeaders();
            foreach (var _ in DayHeaders)
                table.AddColumn(new TableColumn("").Centered());

            // Header row
            table.AddRow(DayHeaders.Select(h => $"[bold]{h}[/]").ToArray());

            // Week rows
            var cursor = startMonday;
            while (cursor <= lastDay)
            {
                var week = Enumerable.Range(0, 7).Select(i => cursor.AddDays(i)).ToList();
                table.AddRow(
                    week.Select(d =>
                        {
                            if (d < firstDay || d > lastDay)
                                return ""; // outside month
                            return $"[grey]{d.Day, 2}[/] {DayCell(d, entryMap, journalDates)}";
                        })
                        .ToArray()
                );
                cursor = cursor.AddDays(7);
            }

            AnsiConsole.Write(table);
        }

        private static string DayCell(
            DateTime day,
            Dictionary<DateTime, MoodEntry> entryMap,
            HashSet<DateTime>? journalDates
        )
        {
            var hasJournal = journalDates?.Contains(day.Date) == true;
            var journalMark = hasJournal ? "[aqua]J[/]" : "";

            if (!entryMap.TryGetValue(day.Date, out var entry))
                return EmptyCell + journalMark;

            if (!Enum.TryParse<Mood>(entry.Mood, out var mood))
                return EmptyCell + journalMark;

            var color = mood.ToSpectreColor();
            return $"[{color}]{BlockCell}[/]{journalMark}";
        }

        private static void RenderLegend()
        {
            AnsiConsole.WriteLine();
            var moods = Enum.GetValues<Mood>();
            var parts = moods.Select(m => $"[{m.ToSpectreColor()}]██[/] {m.ToDisplayName()}");
            AnsiConsole.MarkupLine("Legend: " + string.Join("  ", parts));
            AnsiConsole.WriteLine();
        }

        private static void RenderHint(bool inNavMode = false)
        {
            var hint = inNavMode
                ? "[grey]  ↑↓: navigate   enter: open journal   b: back   esc: exit[/]"
                : "[grey]  d: details   any other key: exit[/]";
            AnsiConsole.MarkupLine(hint);
        }

        private static void RenderDescriptorDetails(
            List<MoodEntry> entries,
            HashSet<DateTime>? journalDates = null,
            int selectedIndex = -1
        )
        {
            var detailEntries = GetDetailEntries(entries, journalDates);

            if (detailEntries.Count == 0)
                return;

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[bold]Details:[/]");

            for (int i = 0; i < detailEntries.Count; i++)
            {
                var entry = detailEntries[i];
                if (!Enum.TryParse<Mood>(entry.Mood, out var mood))
                    continue;

                var color = mood.ToSpectreColor();
                var hasJournal = journalDates?.Contains(entry.Date) == true;
                var journalTag = hasJournal ? " [aqua][[J]][/]" : "";

                string descriptorText;
                if (string.IsNullOrWhiteSpace(entry.Descriptors))
                {
                    descriptorText = "[grey](no descriptors)[/]";
                }
                else
                {
                    var descriptors = string.Join(
                        ", ",
                        entry.Descriptors.Split(
                            ',',
                            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
                        )
                    );
                    descriptorText = $"[aqua]{descriptors}[/]";
                }

                var cursor = i == selectedIndex ? "[bold]>[/] " : "  ";
                AnsiConsole.MarkupLine(
                    $"{cursor}{entry.Date.ToString("ddd, MMM d"), -14}  [{color}]{mood.ToDisplayName(), -12}[/]  {descriptorText}{journalTag}"
                );
            }

            AnsiConsole.WriteLine();
        }

        private static DateTime GetMonday(DateTime date)
        {
            var diff = ((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            return date.AddDays(-diff).Date;
        }
    }
}

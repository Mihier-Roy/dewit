using System;
using System.Collections.Generic;
using Dewit.Core.Entities;
using Spectre.Console;

namespace Dewit.CLI.Utils
{
    public static class Output
    {
        public static bool IsVerbose { get; set; }

        public static void WriteText(string text)
        {
            AnsiConsole.MarkupLine(text);
        }

        public static void WriteError(string text)
        {
            AnsiConsole.MarkupLine($"[red]ERROR[/] : {text}");
        }

        public static void WriteVerbose(string message)
        {
            if (!IsVerbose)
                return;
            AnsiConsole.MarkupLine($"[grey]{Markup.Escape(message)}[/]");
        }

        public static void WriteVerbose(Exception ex, string context)
        {
            if (!IsVerbose)
                return;
            AnsiConsole.MarkupLine(
                $"[grey]{Markup.Escape(context)}: {Markup.Escape(ex.ToString())}[/]"
            );
        }

        public static void WriteTable(
            IAnsiConsole console,
            string[] columnNames,
            IEnumerable<TaskItem> data
        )
        {
            var table = new Table() { Border = TableBorder.Simple };

            foreach (var column in columnNames)
            {
                table.AddColumn(column);
            }

            foreach (var item in data)
            {
                var recurCell =
                    item.RecurringSchedule != null
                        ? $"[blue]↻ {item.RecurringSchedule.ToLabel()}[/]"
                        : "";

                table.AddRow(
                    new string[]
                    {
                        item.Id.ToString(),
                        item.TaskDescription,
                        item.Status == "Done"
                            ? "[green]Done[/]"
                            : (
                                item.Status == "Later" ? "[darkorange]Later[/]" : "[yellow]Doing[/]"
                            ),
                        item.Tags ?? "",
                        item.AddedOn.ToString("dd-MMM-yy HH:mm"),
                        item.CompletedOn == DateTime.Parse("0001-01-01")
                            ? ""
                            : item.CompletedOn.ToString("dd-MMM-yy HH:MM"),
                        recurCell,
                    }
                );
            }

            console.Write(table);
        }
    }
}
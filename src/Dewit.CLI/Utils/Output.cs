using System;
using System.Collections.Generic;
using System.Linq;
using Dewit.Core.Entities;
using Spectre.Console;
using OldTaskItem = Dewit.CLI.Models.TaskItem;

namespace Dewit.CLI.Utils
{
    public static class Output
    {
        public static void WriteText(string text)
        {
            AnsiConsole.MarkupLine(text);
        }

        public static void WriteError(string text)
        {
            AnsiConsole.MarkupLine($"[red]ERROR[/] : {text}");
        }

        public static void WriteTable(string[] columnNames, IEnumerable<TaskItem> data)
        {
            var table = new Table()
            {
                Border = TableBorder.Simple
            };

            foreach (var column in columnNames)
            {
                table.AddColumn(column);
            }

            foreach (var item in data)
            {
                table.AddRow(new string[]
                {
                    item.Id.ToString(),
                    item.TaskDescription,
                    item.Status == "Done" ? "[green]Done[/]" : (item.Status == "Later" ? "[darkorange]Later[/]": "[yellow]Doing[/]"),
                    item.Tags ?? "",
                    item.AddedOn.ToString("dd-MMM-yy HH:mm"),
                    item.CompletedOn == DateTime.Parse("0001-01-01") ? "" : item.CompletedOn.ToString("dd-MMM-yy HH:MM")
                });
            }

            AnsiConsole.Write(table);
        }

        // Overload for old TaskItem type (backward compatibility for import/export commands)
        public static void WriteTable(string[] columnNames, IEnumerable<OldTaskItem> data)
        {
            // Convert old TaskItem to new TaskItem and call the main method
            var newTaskItems = data.Select(old => new TaskItem
            {
                TaskDescription = old.TaskDescription,
                Status = old.Status,
                Tags = old.Tags ?? string.Empty,
                AddedOn = old.AddedOn,
                CompletedOn = old.CompletedOn
            });
            WriteTable(columnNames, newTaskItems);
        }
    }
}

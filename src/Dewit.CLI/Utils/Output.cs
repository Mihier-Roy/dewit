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

                var descCell = string.IsNullOrWhiteSpace(item.Description)
                    ? ""
                    : (item.Description.Length > 50
                        ? item.Description[..50] + "…"
                        : item.Description);

                table.AddRow(
                    new string[]
                    {
                        item.Id.ToString(),
                        item.Title,
                        descCell,
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

        public static void WriteTaskDetail(IAnsiConsole console, TaskItem task)
        {
            var statusMarkup = task.Status switch
            {
                "Done" => "[green]Done[/]",
                "Later" => "[darkorange]Later[/]",
                _ => "[yellow]Doing[/]",
            };

            var recurText =
                task.RecurringSchedule != null
                    ? $"[blue]↻ {task.RecurringSchedule.ToLabel()}[/]"
                    : "[grey]—[/]";

            var completedText =
                task.CompletedOn == DateTime.Parse("0001-01-01")
                    ? "[grey]—[/]"
                    : task.CompletedOn.ToString("dd-MMM-yy HH:mm");

            var grid = new Grid();
            grid.AddColumn(new GridColumn().NoWrap());
            grid.AddColumn(new GridColumn());

            grid.AddRow("[bold]ID[/]", task.Id.ToString());
            grid.AddRow("[bold]Title[/]", Markup.Escape(task.Title));
            grid.AddRow("[bold]Status[/]", statusMarkup);
            grid.AddRow("[bold]Tags[/]", string.IsNullOrEmpty(task.Tags) ? "[grey]—[/]" : Markup.Escape(task.Tags));
            grid.AddRow("[bold]Added[/]", task.AddedOn.ToString("dd-MMM-yy HH:mm"));
            grid.AddRow("[bold]Completed[/]", completedText);
            grid.AddRow("[bold]Recur[/]", recurText);

            var descriptionContent = string.IsNullOrWhiteSpace(task.Description)
                ? "[grey]No description.[/]"
                : Markup.Escape(task.Description);

            var content = new Rows(
                grid,
                new Rule("[grey]Description[/]") { Style = Style.Parse("grey") },
                new Markup(descriptionContent)
            );

            console.Write(
                new Panel(content)
                {
                    Header = new PanelHeader($" Task #{task.Id} "),
                    Border = BoxBorder.Rounded,
                    Padding = new Padding(1, 0),
                }
            );
        }
    }
}
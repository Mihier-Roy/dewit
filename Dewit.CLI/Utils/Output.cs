using System;
using System.Collections.Generic;
using Dewit.CLI.Models;
using Spectre.Console;

namespace Dewit.CLI.Utils
{
	public static class Output
	{
		public static void WriteText(string text)
		{
			AnsiConsole.Markup(text);
		}

		public static void WriteError(string text)
		{
			AnsiConsole.Markup($"[red]ERROR[/] : {text}");
		}

		public static void WriteTable(string[] columnNames, IEnumerable<TaskItem> data)
		{
			var table = new Table();
			table.Border = TableBorder.Square;

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
					item.Status == "Done" ? "[green]Done[/]" : item.Status,
					item.AddedOn.ToString("dd-MMM-yy HH:mm"),
					item.CompletedOn == DateTime.Parse("0001-01-01") ? "" : item.CompletedOn.ToString("dd-MMM-yy HH:MM")
				});
			}

			AnsiConsole.Render(table);
		}
	}
}
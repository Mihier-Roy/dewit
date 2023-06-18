using System;
using System.Collections.Generic;
using Dewit.Core.Entities;
using Dewit.Core.Enums;
using Spectre.Console;

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
			AnsiConsole.MarkupLine($"[red]App Error[/] : {text}");
		}

		public static void WriteConfigTable(IEnumerable<ConfigItem> data)
		{
			var table = new Table()
			{
				Border = TableBorder.Simple
			};
			table.AddColumn("Name");
			table.AddColumn("Value");
			
			foreach (var item in data)
			{
				table.AddRow(((UserConfiguration)item.Id).ToString(), item.Value);
			}

			AnsiConsole.Write(table);
		}

		public static void WriteTasksTable(string[] columnNames, IEnumerable<TaskItem> data)
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
	}
}
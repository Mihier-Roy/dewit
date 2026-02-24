using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using Dewit.CLI.Utils;
using Dewit.Core.Entities;
using Dewit.Core.Interfaces;
using Spectre.Console;

namespace Dewit.CLI.Commands.Task
{
    public class GetTasksCommand : Command
    {
        private readonly ITaskService _taskService;
        private readonly Option<string> _sortOpt;
        private readonly Option<string> _durationOpt;
        private readonly Option<string?> _statusOpt;
        private readonly Option<string?> _tagsOpt;
        private readonly Option<string?> _searchOpt;

        public GetTasksCommand(ITaskService taskService)
            : base("list", "List all your tasks (defaults to today)")
        {
            _taskService = taskService;

            _sortOpt = new Option<string>("--sort")
            {
                Description = "Sort tasks by status or date. Default is <date>",
                DefaultValueFactory = _ => "date",
            };
            _sortOpt.AcceptOnlyFromAmong("status", "date");

            _durationOpt = new Option<string>("--duration")
            {
                Description =
                    "Show tasks between the specified duration. Default is set to <today>.",
                DefaultValueFactory = _ => "today",
            };
            _durationOpt.AcceptOnlyFromAmong("all", "yesterday", "today", "week", "month");

            _statusOpt = new Option<string?>("--status")
            {
                Description = "Show tasks of specified status.",
            };
            _statusOpt.AcceptOnlyFromAmong("doing", "done", "later");

            _tagsOpt = new Option<string?>("--tags")
            {
                Description = "Filter tasks based on tags.",
            };
            _searchOpt = new Option<string?>("--search")
            {
                Description = "Search for tasks that contain the input string.",
            };

            Options.Add(_sortOpt);
            Options.Add(_durationOpt);
            Options.Add(_statusOpt);
            Options.Add(_tagsOpt);
            Options.Add(_searchOpt);

            SetAction(parseResult =>
            {
                var sort = parseResult.GetValue(_sortOpt)!;
                var duration = parseResult.GetValue(_durationOpt)!;
                var status = parseResult.GetValue(_statusOpt);
                var tags = parseResult.GetValue(_tagsOpt);
                var search = parseResult.GetValue(_searchOpt);
                GetTasks(sort, duration, status, tags, search);
            });
        }

        private void GetTasks(
            string sort,
            string duration,
            string? status = null,
            string? tags = null,
            string? search = null
        )
        {
            try
            {
                var tasks = _taskService.GetTasks(sort, duration, status, tags, search).ToList();

                Output.WriteText(
                    $"Displaying tasks using parameters -> [aqua]sort[/]: {sort}, [aqua]duration[/] : {duration}, [aqua]status[/]: {status ?? "n/a"}, [aqua]tags[/]:{tags}"
                );

                if (tasks.Count == 0)
                {
                    Output.WriteText("[grey]No tasks found.[/]");
                    return;
                }

                if (AnsiConsole.Console.Profile.Capabilities.Interactive)
                {
                    RunInteractiveList(tasks);
                }
                else
                {
                    Output.WriteTable(
                        AnsiConsole.Console,
                        new string[] { "ID", "Task", "Description", "Status", "Tags", "Added On", "Completed On", "Recur" },
                        tasks
                    );
                }
            }
            catch (Exception ex)
            {
                Output.WriteVerbose(ex, "Failed to retrieve tasks");
                Output.WriteError("Failed to retrieve tasks. Please try again.");
            }
        }

        private static void RunInteractiveList(List<TaskItem> tasks)
        {
            while (true)
            {
                AnsiConsole.Clear();

                Output.WriteTable(
                    AnsiConsole.Console,
                    new string[] { "ID", "Task", "Description", "Status", "Tags", "Added On", "Completed On", "Recur" },
                    tasks
                );

                var selectedId = ReadTaskId();
                if (selectedId == null)
                    break;

                var selectedTask = tasks.FirstOrDefault(t => t.Id == selectedId);
                if (selectedTask == null)
                {
                    AnsiConsole.MarkupLine($"[red]No task with ID {selectedId} in the current list.[/]");
                    System.Threading.Thread.Sleep(800);
                    continue;
                }

                AnsiConsole.Clear();
                Output.WriteTaskDetail(AnsiConsole.Console, selectedTask);
                AnsiConsole.MarkupLine("[grey]Press any key to return to the list...[/]");
                Console.ReadKey(intercept: true);
            }
        }

        private static int? ReadTaskId()
        {
            AnsiConsole.Markup("[grey]Enter ID to view, or any other key to exit: [/]");

            var digits = new System.Text.StringBuilder();

            while (true)
            {
                var key = Console.ReadKey(intercept: true);

                if (char.IsDigit(key.KeyChar))
                {
                    digits.Append(key.KeyChar);
                    Console.Write(key.KeyChar);
                }
                else if (key.Key == ConsoleKey.Enter && digits.Length > 0)
                {
                    Console.WriteLine();
                    return int.Parse(digits.ToString());
                }
                else if (key.Key == ConsoleKey.Backspace && digits.Length > 0)
                {
                    digits.Remove(digits.Length - 1, 1);
                    Console.Write("\b \b");
                }
                else
                {
                    Console.WriteLine();
                    return null;
                }
            }
        }
    }
}
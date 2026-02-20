using System;
using System.CommandLine;
using Dewit.CLI.Utils;
using Dewit.Core.Interfaces;
using Serilog;

namespace Dewit.CLI.Commands
{
    public class GetTasksCommand : Command
    {
        private readonly ITaskService _taskService;
        private readonly Option<string> _sortOpt;
        private readonly Option<string> _durationOpt;
        private readonly Option<string?> _statusOpt;
        private readonly Option<string?> _tagsOpt;
        private readonly Option<string?> _searchOpt;

        public GetTasksCommand(ITaskService taskService, string name, string? description = null)
            : base(name, description)
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

            this.Options.Add(_sortOpt);
            this.Options.Add(_durationOpt);
            this.Options.Add(_statusOpt);
            this.Options.Add(_tagsOpt);
            this.Options.Add(_searchOpt);

            this.SetAction(parseResult =>
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
                var tasks = _taskService.GetTasks(sort, duration, status, tags, search);

                Output.WriteText(
                    $"Displaying tasks using parameters -> [aqua]sort[/]: {sort}, [aqua]duration[/] : {duration}, [aqua]status[/]: {status ?? "n/a"}, [aqua]tags[/]:{tags}"
                );
                Output.WriteTable(
                    new string[] { "ID", "Task", "Status", "Tags", "Added On", "Completed On" },
                    tasks
                );
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to retrieve tasks");
                Output.WriteError("Failed to retrieve tasks. Please try again.");
            }
        }
    }
}

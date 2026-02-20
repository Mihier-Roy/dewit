using System;
using System.CommandLine;
using System.Linq;
using Dewit.CLI.Utils;
using Dewit.Core.Enums;
using Dewit.Core.Interfaces;
using Spectre.Console;
using MoodEnum = Dewit.Core.Enums.Mood;

namespace Dewit.CLI.Commands.Config
{
    public class DescriptorsListCommand : Command
    {
        private readonly IMoodService _moodService;

        public DescriptorsListCommand(IMoodService moodService)
            : base("list", "List descriptors for all moods.")
        {
            _moodService = moodService;
            SetAction(_ => Run());
        }

        private void Run()
        {
            var items = _moodService.GetAllDescriptors().ToList();

            if (items.Count == 0)
            {
                Output.WriteText("[grey]No mood descriptors found.[/]");
                return;
            }

            var table = new Table { Border = TableBorder.Simple };
            table.AddColumn("Mood");
            table.AddColumn("Descriptors");

            foreach (var item in items)
            {
                var color = MoodExtensions.ToSpectreColor(
                    Enum.Parse<MoodEnum>(item.Mood, ignoreCase: true)
                );
                table.AddRow($"[{color}]{item.Mood}[/]", item.Descriptors);
            }

            AnsiConsole.Write(table);
        }
    }
}

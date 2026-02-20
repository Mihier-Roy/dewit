using System;
using System.CommandLine;
using Dewit.CLI.Utils;
using Dewit.Core.Enums;
using Dewit.Core.Interfaces;
using MoodEnum = Dewit.Core.Enums.Mood;

namespace Dewit.CLI.Commands.Config
{
    public class DescriptorsResetCommand : Command
    {
        private readonly IMoodService _moodService;
        private readonly Argument<string?> _moodArg;
        private readonly Option<bool> _allOpt;

        public DescriptorsResetCommand(IMoodService moodService)
            : base("reset", "Reset descriptors for a mood (or all moods) to defaults.")
        {
            _moodService = moodService;

            _moodArg = new Argument<string?>("mood")
            {
                Description = "The mood to reset. Omit to use --all.",
                Arity = ArgumentArity.ZeroOrOne,
            };
            _allOpt = new Option<bool>("--all")
            {
                Description = "Reset descriptors for all moods.",
            };

            this.Arguments.Add(_moodArg);
            this.Options.Add(_allOpt);

            this.SetAction(parseResult =>
            {
                var mood = parseResult.GetValue(_moodArg);
                var all = parseResult.GetValue(_allOpt);
                Run(mood, all);
            });
        }

        private void Run(string? mood, bool all)
        {
            if (all)
            {
                foreach (var m in Enum.GetValues<MoodEnum>())
                    _moodService.ResetDescriptors(m.ToString());
                Output.WriteText("[green]All mood descriptors reset to defaults.[/]");
                return;
            }

            if (string.IsNullOrWhiteSpace(mood))
            {
                Output.WriteError("Provide a mood name or use --all to reset all moods.");
                return;
            }

            if (!MoodExtensions.TryParse(mood, out var moodEnum))
            {
                Output.WriteError(
                    $"Unknown mood '{mood}'. Valid moods: VeryHappy, Happy, Meh, Down, ExtraDown."
                );
                return;
            }

            _moodService.ResetDescriptors(moodEnum.ToString());
            Output.WriteText(
                $"[green]Descriptors reset[/] for [bold]{moodEnum.ToDisplayName()}[/]."
            );
        }
    }
}

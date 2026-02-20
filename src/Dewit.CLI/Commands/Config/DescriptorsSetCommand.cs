using System.CommandLine;
using Dewit.CLI.Utils;
using Dewit.Core.Enums;
using Dewit.Core.Interfaces;

namespace Dewit.CLI.Commands.Config
{
    public class DescriptorsSetCommand : Command
    {
        private readonly IMoodService _moodService;
        private readonly Argument<string> _moodArg;
        private readonly Argument<string> _descriptorsArg;

        public DescriptorsSetCommand(IMoodService moodService)
            : base("set", "Set descriptors for a mood (comma-separated list).")
        {
            _moodService = moodService;

            _moodArg = new Argument<string>("mood")
            {
                Description = "The mood to update (e.g. Happy, VeryHappy, Meh, Down, ExtraDown).",
            };
            _descriptorsArg = new Argument<string>("descriptors")
            {
                Description = "Comma-separated list of descriptors.",
            };

            this.Arguments.Add(_moodArg);
            this.Arguments.Add(_descriptorsArg);

            this.SetAction(parseResult =>
            {
                var mood = parseResult.GetValue(_moodArg)!;
                var descriptors = parseResult.GetValue(_descriptorsArg)!;
                Run(mood, descriptors);
            });
        }

        private void Run(string mood, string descriptors)
        {
            if (!MoodExtensions.TryParse(mood, out var moodEnum))
            {
                Output.WriteError(
                    $"Unknown mood '{mood}'. Valid moods: VeryHappy, Happy, Meh, Down, ExtraDown."
                );
                return;
            }

            _moodService.SetDescriptors(moodEnum.ToString(), descriptors);
            Output.WriteText(
                $"[green]Descriptors updated[/] for [bold]{moodEnum.ToDisplayName()}[/]."
            );
        }
    }
}

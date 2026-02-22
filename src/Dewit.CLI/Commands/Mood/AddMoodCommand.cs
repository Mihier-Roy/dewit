using System;
using System.CommandLine;
using System.Linq;
using Dewit.CLI.Utils;
using Dewit.Core.Enums;
using Dewit.Core.Interfaces;
using Spectre.Console;
using MoodEnum = Dewit.Core.Enums.Mood;

namespace Dewit.CLI.Commands.Mood
{
    public class AddMoodCommand : Command
    {
        private readonly IMoodService _moodService;
        private readonly Option<string?> _moodOpt;
        private readonly Option<string?> _descriptorsOpt;

        public AddMoodCommand(IMoodService moodService)
            : base("add", "Log your mood for today.")
        {
            _moodService = moodService;

            _moodOpt = new Option<string?>("--mood")
            {
                Description = "Your mood: veryhappy, happy, meh, down, extradown",
            };
            _descriptorsOpt = new Option<string?>("--descriptors")
            {
                Description = "Comma-separated descriptors, e.g. --descriptors calm,focused",
            };

            Options.Add(_moodOpt);
            Options.Add(_descriptorsOpt);

            SetAction(parseResult =>
            {
                var moodInput = parseResult.GetValue(_moodOpt);
                var descriptorsInput = parseResult.GetValue(_descriptorsOpt);
                Run(moodInput, descriptorsInput);
            });
        }

        private void Run(string? moodInput, string? descriptorsInput)
        {
            try
            {
                var today = DateTime.Today;
                var existing = _moodService.GetEntryForDate(today);

                if (existing != null)
                {
                    Output.WriteText(
                        $"[yellow]You already logged a mood for today:[/] {existing.Mood}"
                    );
                    var update = AnsiConsole.Confirm("Would you like to update it instead?");
                    if (!update)
                        return;

                    var (mood, descriptors) = ResolveMoodAndDescriptors(
                        moodInput,
                        descriptorsInput,
                        existing.Mood
                    );
                    _moodService.UpdateEntry(today, mood, descriptors);
                    Output.WriteText($"[green]Updated today's mood to[/] {mood}.");
                    return;
                }

                var (selectedMood, selectedDescriptors) = ResolveMoodAndDescriptors(
                    moodInput,
                    descriptorsInput,
                    null
                );
                _moodService.AddEntry(selectedMood, selectedDescriptors, today);
                Output.WriteText(
                    $"[green]Logged mood:[/] {selectedMood}"
                        + (
                            string.IsNullOrEmpty(selectedDescriptors)
                                ? ""
                                : $" | [aqua]{selectedDescriptors}[/]"
                        )
                );
            }
            catch (Exception ex)
            {
                Output.WriteVerbose(ex, "Failed to add mood entry");
                Output.WriteError("Failed to log mood. Please try again.");
            }
        }

        private (string mood, string descriptors) ResolveMoodAndDescriptors(
            string? moodInput,
            string? descriptorsInput,
            string? currentMood
        )
        {
            string mood;

            if (moodInput != null && MoodExtensions.TryParse(moodInput, out var parsedMood))
            {
                mood = parsedMood.ToString();
            }
            else
            {
                // Interactive mood selection
                var prompt = new SelectionPrompt<MoodEnum>()
                    .Title("How are you feeling [bold]today[/]?")
                    .UseConverter(m => m.ToDisplayName());

                foreach (var m in Enum.GetValues<MoodEnum>())
                    prompt.AddChoice(m);

                mood = AnsiConsole.Prompt(prompt).ToString();
            }

            string descriptors;

            if (descriptorsInput != null)
            {
                descriptors = descriptorsInput;
            }
            else
            {
                // Interactive descriptor selection
                var available = _moodService.GetDescriptors(mood).ToList();
                if (available.Count == 0)
                {
                    descriptors = string.Empty;
                }
                else
                {
                    var multiPrompt = new MultiSelectionPrompt<string>()
                        .Title(
                            $"What's contributing to feeling [bold]{mood}[/]? (space to select, enter to confirm)"
                        )
                        .NotRequired()
                        .AddChoices(available);

                    var selected = AnsiConsole.Prompt(multiPrompt);
                    descriptors = string.Join(',', selected);
                }
            }

            return (mood, descriptors);
        }
    }
}
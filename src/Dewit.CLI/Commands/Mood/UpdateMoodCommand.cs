using System;
using System.CommandLine;
using System.Linq;
using Dewit.CLI.Utils;
using Dewit.Core.Enums;
using Dewit.Core.Interfaces;
using Dewit.Core.Utils;
using Spectre.Console;
using MoodEnum = Dewit.Core.Enums.Mood;

namespace Dewit.CLI.Commands.Mood
{
    public class UpdateMoodCommand : Command
    {
        private readonly IMoodService _moodService;
        private readonly Option<string?> _dateOpt;
        private readonly Option<string?> _moodOpt;
        private readonly Option<string?> _descriptorsOpt;

        public UpdateMoodCommand(IMoodService moodService)
            : base("update", "Update a mood entry. Defaults to today.")
        {
            _moodService = moodService;

            _dateOpt = new Option<string?>("--date")
            {
                Description =
                    "Date to update. Accepts: today, yesterday, last monday, YYYY-MM-DD, MM-DD. Defaults to today.",
            };
            _moodOpt = new Option<string?>("--mood")
            {
                Description = "New mood: veryhappy, happy, meh, down, extradown",
            };
            _descriptorsOpt = new Option<string?>("--descriptors")
            {
                Description = "New comma-separated descriptors",
            };

            Options.Add(_dateOpt);
            Options.Add(_moodOpt);
            Options.Add(_descriptorsOpt);

            SetAction(parseResult =>
            {
                var dateInput = parseResult.GetValue(_dateOpt);
                var moodInput = parseResult.GetValue(_moodOpt);
                var descriptorsInput = parseResult.GetValue(_descriptorsOpt);
                Run(dateInput, moodInput, descriptorsInput);
            });
        }

        private void Run(string? dateInput, string? moodInput, string? descriptorsInput)
        {
            try
            {
                DateTime date;
                try
                {
                    date = dateInput == null ? DateTime.Today : DateParser.Parse(dateInput);
                }
                catch (ArgumentException ex)
                {
                    Output.WriteError(ex.Message);
                    return;
                }

                var existing = _moodService.GetEntryForDate(date);
                if (existing == null)
                {
                    Output.WriteError(
                        $"No mood entry found for {date:yyyy-MM-dd}. Use 'mood add' to create one."
                    );
                    return;
                }

                Output.WriteText(
                    $"Current entry for [aqua]{date:yyyy-MM-dd}[/]: Mood=[bold]{existing.Mood}[/] | Descriptors={existing.Descriptors}"
                );

                // Resolve mood
                string? mood = null;
                if (moodInput != null)
                {
                    if (!MoodExtensions.TryParse(moodInput, out var parsed))
                    {
                        Output.WriteError($"Unknown mood '{moodInput}'.");
                        return;
                    }
                    mood = parsed.ToString();
                }
                else if (!AnsiConsole.Confirm("Keep current mood?", defaultValue: true))
                {
                    var prompt = new SelectionPrompt<MoodEnum>()
                        .Title("Select new mood:")
                        .UseConverter(m => m.ToDisplayName());
                    foreach (var m in Enum.GetValues<MoodEnum>())
                        prompt.AddChoice(m);
                    mood = AnsiConsole.Prompt(prompt).ToString();
                }

                // Resolve descriptors
                string? descriptors = null;
                if (descriptorsInput != null)
                {
                    descriptors = descriptorsInput;
                }
                else if (!AnsiConsole.Confirm("Keep current descriptors?", defaultValue: true))
                {
                    var moodForDescriptors = mood ?? existing.Mood;
                    var available = _moodService.GetDescriptors(moodForDescriptors).ToList();
                    if (available.Count > 0)
                    {
                        var multiPrompt = new MultiSelectionPrompt<string>()
                            .Title("Select new descriptors:")
                            .NotRequired()
                            .AddChoices(available);
                        descriptors = string.Join(',', AnsiConsole.Prompt(multiPrompt));
                    }
                }

                _moodService.UpdateEntry(date, mood, descriptors);
                Output.WriteText($"[green]Updated mood entry for {date:yyyy-MM-dd}.[/]");
            }
            catch (Exception ex)
            {
                Output.WriteVerbose(ex, "Failed to update mood entry");
                Output.WriteError("Failed to update mood entry. Please try again.");
            }
        }
    }
}
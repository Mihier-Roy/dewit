using System;
using System.CommandLine;
using System.Linq;
using Dewit.CLI.Utils;
using Dewit.Core.Enums;
using Dewit.Core.Interfaces;
using Spectre.Console;
using MoodEnum = Dewit.Core.Enums.Mood;

namespace Dewit.CLI.Commands.Journal
{
    public class AddJournalCommand : Command
    {
        private readonly IMoodService _moodService;
        private readonly IJournalService _journalService;
        private readonly Option<string?> _moodOpt;
        private readonly Option<string?> _descriptorsOpt;

        public AddJournalCommand(IMoodService moodService, IJournalService journalService)
            : base("add", "Log your mood for today. Optionally add a journal entry.")
        {
            _moodService = moodService;
            _journalService = journalService;

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
                    PromptJournal(today, mood, descriptors);
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

                PromptJournal(today, selectedMood, selectedDescriptors);
            }
            catch (Exception ex)
            {
                Output.WriteVerbose(ex, "Failed to add mood/journal entry");
                Output.WriteError("Failed to log mood. Please try again.");
            }
        }

        private void PromptJournal(DateTime date, string mood, string descriptors)
        {
            if (!AnsiConsole.Confirm("Would you like to add a journal entry?", defaultValue: false))
                return;

            var entry = _journalService.CreateOrGetEntry(date, mood, descriptors);
            Output.WriteText($"[grey]Opening {Markup.Escape(entry.FilePath)}...[/]");
            EditorHelper.Open(entry.FilePath);
            _journalService.TouchUpdatedAt(date);
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
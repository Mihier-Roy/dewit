using System.CommandLine;
using Dewit.Core.Interfaces;

namespace Dewit.CLI.Commands.Mood
{
    public class MoodCommand : Command
    {
        public MoodCommand(IMoodService moodService)
            : base("mood", "Track and view your daily mood.")
        {
            Subcommands.Add(new AddMoodCommand(moodService));
            Subcommands.Add(new UpdateMoodCommand(moodService));
            Subcommands.Add(new ViewMoodCommand(moodService));
        }
    }
}
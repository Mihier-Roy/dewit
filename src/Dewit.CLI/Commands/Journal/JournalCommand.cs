using System.CommandLine;
using Dewit.Core.Interfaces;

namespace Dewit.CLI.Commands.Journal
{
    public class JournalCommand : Command
    {
        public JournalCommand(IMoodService moodService, IJournalService journalService)
            : base("journal", "Log and review your mood and journal entries.")
        {
            Subcommands.Add(new AddJournalCommand(moodService, journalService));
            Subcommands.Add(new UpdateJournalCommand(moodService, journalService));
            Subcommands.Add(new ViewJournalCommand(moodService, journalService));
        }
    }
}

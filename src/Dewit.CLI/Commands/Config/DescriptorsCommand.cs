using System.CommandLine;
using Dewit.Core.Interfaces;

namespace Dewit.CLI.Commands.Config
{
    public class DescriptorsCommand : Command
    {
        public DescriptorsCommand(IMoodService moodService)
            : base("descriptors", "View and manage mood descriptors.")
        {
            Subcommands.Add(new DescriptorsListCommand(moodService));
            Subcommands.Add(new DescriptorsSetCommand(moodService));
            Subcommands.Add(new DescriptorsResetCommand(moodService));
        }
    }
}

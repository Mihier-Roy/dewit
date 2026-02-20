using System.CommandLine;
using Dewit.Core.Interfaces;

namespace Dewit.CLI.Commands.Config
{
    public class DescriptorsCommand : Command
    {
        public DescriptorsCommand(IMoodService moodService)
            : base("descriptors", "View and manage mood descriptors.")
        {
            this.Subcommands.Add(new DescriptorsListCommand(moodService));
            this.Subcommands.Add(new DescriptorsSetCommand(moodService));
            this.Subcommands.Add(new DescriptorsResetCommand(moodService));
        }
    }
}

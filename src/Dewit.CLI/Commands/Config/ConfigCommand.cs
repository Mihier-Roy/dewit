using System.CommandLine;
using Dewit.Core.Interfaces;

namespace Dewit.CLI.Commands.Config
{
    public class ConfigCommand : Command
    {
        public ConfigCommand(IConfigurationService configService, IMoodService moodService)
            : base("config", "View and manage application configuration.")
        {
            this.Subcommands.Add(new ConfigListCommand(configService));
            this.Subcommands.Add(new ConfigSetCommand(configService));
            this.Subcommands.Add(new DescriptorsCommand(moodService));
        }
    }
}

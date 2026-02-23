using System.CommandLine;
using Dewit.Core.Interfaces;

namespace Dewit.CLI.Commands.Config
{
    public class ConfigCommand : Command
    {
        public ConfigCommand(IConfigurationService configService, IMoodService moodService)
            : base("config", "View and manage application configuration.")
        {
            Subcommands.Add(new ConfigListCommand(configService));
            Subcommands.Add(new ConfigSetCommand(configService));
            Subcommands.Add(new DescriptorsCommand(moodService));
        }
    }
}
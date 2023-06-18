using System.ComponentModel;
using Dewit.CLI.Utils;
using Dewit.Core.Enums;
using Dewit.Core.Interfaces;
using Dewit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Spectre.Console.Cli;

namespace Dewit.CLI.Branches.Config;

public class ConfigCommand : Command<ConfigCommand.Settings>
{
    private readonly IConfigurationService _configurationService;

    public class Settings : CommandSettings
    {
    }

    public ConfigCommand(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }
    
    public override int Execute(CommandContext context, Settings settings)
    {
        // Print current config at the end of the command
        var data = _configurationService.ListValues();
            Output.WriteText(":information: Here is your current configuration:");
            Output.WriteConfigTable(data);
            Output.WriteText("Run [blue]dewit --help[/] for a list of available command");

            return 0;
    }
}
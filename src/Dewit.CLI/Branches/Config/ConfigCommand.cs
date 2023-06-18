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
    private readonly DewitDbContext _dbContext;
    private readonly IConfigurationService _configurationService;

    public class Settings : CommandSettings
    {
        [CommandOption("--first-run <first-run>", IsHidden = true)]
        [Description("Indicate if first run configuration is to be performed")]
        public bool FirstRun { get; set; }
    }

    public ConfigCommand(DewitDbContext dbContext, IConfigurationService configurationService)
    {
        _dbContext = dbContext;
        _configurationService = configurationService;
    }
    
    public override int Execute(CommandContext context, Settings settings)
    {
        if (settings.FirstRun)
        {
            Output.WriteText(":light_bulb: Detected that app is running for first time");
            Output.WriteText(":wrench: Configuring app for first time use");
            try
            {
                _dbContext.Database.Migrate();
                Output.WriteText(
                    ":check_mark: [green]Application has been configured![/]\nRun [blue]dewit --help[/] for usage information.");
                _configurationService.SetValue((int)UserConfiguration.FirstRun, "false");
            }
            catch
            {
                Output.WriteError("Error configuring database");
            }
        }
        else
        {
            Output.WriteText(
                ":check_mark: [grey]Configuration has not been changed![/]\nRun [blue]dewit --help[/] for usage information.");
        }

        return 0;
    }
}
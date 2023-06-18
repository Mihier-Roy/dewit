using System.ComponentModel;
using Dewit.CLI.Utils;
using Dewit.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Spectre.Console.Cli;

namespace Dewit.CLI.Branches.Setup;

public class SetupCommand : Command<SetupCommand.Settings>
{
    private readonly DewitDbContext _dbContext;

    public class Settings : CommandSettings
    {
        [CommandOption("--first-run <first-run>", IsHidden = true)]
        [Description("Indicate if first run configuration is to be performed")]
        public bool FirstRun { get; set; }
    }

    public SetupCommand(DewitDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public override int Execute(CommandContext context, Settings settings)
    {
        if (settings.FirstRun)
        {
            Output.WriteText(":light_bulb:  Detected that app is running for first time");
            Output.WriteText(":wrench:  Configuring app for first time use");
        }
        
        // Apply migrations, this also creates the database
        try
        {
            _dbContext.Database.Migrate();
            Output.WriteText(":check_mark:  Application has been configured!  :rocket: \nRun [blue]dewit --help[/] for usage information");
        }
        catch
        {
            Output.WriteError("Error configuring database. Please try again");
        }

        return 0;
    }
}
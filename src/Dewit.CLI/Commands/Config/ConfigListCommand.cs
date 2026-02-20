using System.CommandLine;
using System.Linq;
using Dewit.CLI.Utils;
using Dewit.Core.Interfaces;
using Spectre.Console;

namespace Dewit.CLI.Commands.Config
{
    public class ConfigListCommand : Command
    {
        private readonly IConfigurationService _configService;

        public ConfigListCommand(IConfigurationService configService)
            : base("list", "List all configuration values.")
        {
            _configService = configService;
            SetAction(_ => Run());
        }

        private void Run()
        {
            var items = _configService.GetAll().OrderBy(kv => kv.Key).ToList();

            if (items.Count == 0)
            {
                Output.WriteText("[grey]No configuration entries found.[/]");
                return;
            }

            var table = new Table { Border = TableBorder.Simple };
            table.AddColumn("Key");
            table.AddColumn("Value");

            foreach (var kv in items)
                table.AddRow(kv.Key, kv.Value);

            AnsiConsole.Write(table);
        }
    }
}

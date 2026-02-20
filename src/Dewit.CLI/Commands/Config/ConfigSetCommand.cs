using System.CommandLine;
using Dewit.CLI.Utils;
using Dewit.Core.Interfaces;

namespace Dewit.CLI.Commands.Config
{
    public class ConfigSetCommand : Command
    {
        private readonly IConfigurationService _configService;
        private readonly Argument<string> _keyArg;
        private readonly Argument<string> _valueArg;

        public ConfigSetCommand(IConfigurationService configService) : base("set", "Set a configuration value.")
        {
            _configService = configService;

            _keyArg = new Argument<string>("key") { Description = "The configuration key to set." };
            _valueArg = new Argument<string>("value") { Description = "The value to assign." };

            this.Arguments.Add(_keyArg);
            this.Arguments.Add(_valueArg);

            this.SetAction(parseResult =>
            {
                var key = parseResult.GetValue(_keyArg)!;
                var value = parseResult.GetValue(_valueArg)!;
                Run(key, value);
            });
        }

        private void Run(string key, string value)
        {
            _configService.SetValue(key, value);
            Output.WriteText($"[green]Config updated.[/] {key} = {value}");
        }
    }
}

using CommandComplete.ConsoleParsing;
using System;

namespace CommandComplete.ConsoleTestClient
{
    public class Program
    {
        static void Main(string[] args)
        {
            var commandCache = new CommandCache();
            var consoleCommandLineParser = new ConsoleCommandLineParser();
            var console = new CommandingConsole();

            InitCommands(commandCache);

            var result = consoleCommandLineParser.ParseCommandLine(commandCache, console);

            Console.WriteLine($"Chosen Command: {result.Command.Name}");

            Console.WriteLine("Flagged Params:");
            foreach (var flaggedParam in result.FlaggedParameters)
            {
                Console.WriteLine($"\t{flaggedParam.Name}");
            }

            Console.WriteLine("Valued Params:");
            foreach (var (parameter, value) in result.ValuedParameters)
            {
                Console.WriteLine($"\t{parameter.Name}: {value}");
            }

            Console.WriteLine($"Remaining Text: {result.RemainingText}");
        }

        private static void InitCommands(ICommandCache commandCache)
        {
            var commandNames = new[] { "Help", "Exit", "Command1", "Command2", "Command3" };

            foreach (var commandName in commandNames)
            {
                var parameters = new[] {
                    new ParameterOption("Param1", true, "Parameter 1"),
                    new ParameterOption("Param2", true, "Parameter 2"),
                    new ParameterOption("Param3", true, "Parameter 3"),

                    new ParameterOption("Flag1", false, "Flag 1"),
                    new ParameterOption("Flag2", false, "Flag 2"),
                };
                var command = new Command(commandName, '-', string.Empty, parameters);

                commandCache.AddCommand(command);
            }
        }

    }
}

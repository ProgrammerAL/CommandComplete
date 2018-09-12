using CommandComplete.ConsoleParsing;
using System;

namespace CommandComplete.ConsoleTestClient
{
    static class Program
    {
        static void Main(string[] args)
        {
            var commandCache = new CommandCache();
            var consoleCommandLineParser = new ConsoleCommandLineParser();
            var console = new CommandingConsole();

            InitCommands(commandCache);

            while (true)
            {
                Console.WriteLine("Enter a command: ");
                ParseCommandLineResult result = consoleCommandLineParser.ParseCommandLine(commandCache, console);

                if (!result.ThinkWeHaveSomething)
                {
                    Console.WriteLine("No command parsed from string: " + result.ToString());
                }
                else
                {
                    Console.WriteLine($"Chosen Command: {result.Command.Name}");

                    Console.WriteLine("Flagged Params:");
                    foreach (ParameterOption flaggedParam in result.FlaggedParameters)
                    {
                        Console.WriteLine($"\t{flaggedParam.Name}");
                    }

                    Console.WriteLine("Valued Params:");
                    foreach ((ParameterOption parameter, string value) in result.ValuedParameters)
                    {
                        Console.WriteLine($"\t{parameter.Name}: {value}");
                    }

                    Console.WriteLine($"Remaining Text: {result.RemainingText}");
                }
            }
        }

        private static void InitCommands(ICommandCache commandCache)
        {
            string[] commandNames = new[] { "Help", "Exit", "Command1", "Command2", "Command3" };

            foreach (string commandName in commandNames)
            {
                ParameterOption[] parameters = new[] {
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

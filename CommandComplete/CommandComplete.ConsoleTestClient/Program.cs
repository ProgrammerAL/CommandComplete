using CommandComplete.ConsoleParsing;
using System;

namespace CommandComplete.ConsoleTestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var commandCache = new CommandCache();
            var consoleCommandLineParser = new ConsoleCommandLineParser();
            var console = new CommandingConsole();

            InitCommands(commandCache);

            var consoleCommandLineResult = consoleCommandLineParser.ParseCommandLine(commandCache, console);

            Console.WriteLine($"Chosen Command: {consoleCommandLineResult.Command.Name}");
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
                };
                var command = new Command(commandName, '-', string.Empty, parameters);

                commandCache.AddCommand(command);
            }
        }

    }
}

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
                var parameters = Array.Empty<ParameterOption>();
                var command = new Command(commandName, '-', string.Empty, parameters);

                commandCache.AddCommand(command);
            }
        }

    }
}

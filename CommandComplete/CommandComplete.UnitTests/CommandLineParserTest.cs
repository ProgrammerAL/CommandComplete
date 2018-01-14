using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;
using CommandComplete;

namespace CommandComplete.UnitTests
{
    public class CommandLineParserTest
    {
        [Fact]
        public void WhenParsingCommandWithParams_AssertResult()
        {
            var commandCache = GenerateCommandCache();
            var parser = new CommandLineParser();
            var result = parser.ParseCommandLine("command1 -flagParam1 -param1 value1 -para", ConsoleKey.M, null, commandCache);

            Assert.True(result.ThinkWeHaveSomething);
            Assert.Equal("command1", result.Command.Name, ignoreCase: true);
            Assert.Equal("FlagParam1", result.FlaggedParameters.Single().Name);
            Assert.Equal("Param1", result.ValuedParameters.Single().Parameter.Name);

            Assert.Equal(0, result.TabbedCount);
            Assert.Equal("-para", result.RemainingText);

            Assert.Equal(2, result.PossibleTextsToAutofill.Count);
        }

        [Fact]
        public void WhenPressingTabAfterCommanding_AssertTabbedCountIncremented()
        {
            var commandCache = GenerateCommandCache();
            var parser = new CommandLineParser();
            var result = parser.ParseCommandLine("command1 -flagParam1 -param1 value1 -para", ConsoleKey.M, null, commandCache);
            result = parser.ParseCommandLine("command1 -flagParam1 -param1 value1 -param", ConsoleKey.Tab, result, commandCache);
            Assert.Equal(1, result.TabbedCount);

            result = parser.ParseCommandLine("command1 -flagParam1 -param1 value1 -param", ConsoleKey.Tab, result, commandCache);
            Assert.Equal(2, result.TabbedCount);
        }

        private ICommandCache GenerateCommandCache()
        {
            var cache = new CommandCache();

            var command1 = new Command("Command1", '-', "This is command 1 for use in unit tests",
            new[]
            {
                new ParameterOption("Param1", true, "Parameter 1"),
                new ParameterOption("Param2", true, "Parameter 2"),
                new ParameterOption("Param3", true, "Parameter 3"),
                new ParameterOption("FlagParam1", false, "Flagged Parameter 1"),
                new ParameterOption("FlagParam2", false, "Flagged Parameter 2"),
            });

            var command2 = new Command("Command2", '-', "This is command 2 for use in unit tests",
            new[]
            {
                new ParameterOption("Param1", true, "Parameter 1"),
                new ParameterOption("Param2", true, "Parameter 2"),
                new ParameterOption("Param3", true, "Parameter 3"),
                new ParameterOption("FlagParam1", false, "Flagged Parameter 1"),
                new ParameterOption("FlagParam2", false, "Flagged Parameter 2"),
            });

            cache.AddCommand(command1);
            cache.AddCommand(command2);

            return cache;
        }
    }
}

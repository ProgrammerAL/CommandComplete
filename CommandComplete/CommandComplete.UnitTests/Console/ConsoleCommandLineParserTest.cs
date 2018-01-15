using CommandComplete.ConsoleParsing;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;

namespace CommandComplete.UnitTests.Console
{
    public class ConsoleCommandLineParserTest
    {
        private readonly CommandingConsoleFake _commandingConsoleSubstitute;

        public ConsoleCommandLineParserTest()
        {
            _commandingConsoleSubstitute = new CommandingConsoleFake();
        }

        [Fact]
        public void WhenParsingInput_AssertDirectResult()
        {
            var commandCache = GenerateCommandCache();

            _commandingConsoleSubstitute.SetConsoleKeyInfoOrder(new[]
            {
                new ConsoleKeyInfo('C', ConsoleKey.C, true, false, false),
                new ConsoleKeyInfo('o', ConsoleKey.O, false, false, false),
                new ConsoleKeyInfo('m', ConsoleKey.M, false, false, false),
                new ConsoleKeyInfo('m', ConsoleKey.M, false, false, false),
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),
                new ConsoleKeyInfo('n', ConsoleKey.N, false, false, false),
                new ConsoleKeyInfo('d', ConsoleKey.D, false, false, false),
                new ConsoleKeyInfo('1', ConsoleKey.NumPad1, false, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, false, false, false),

                new ConsoleKeyInfo('-', ConsoleKey.OemMinus, false, false, false),
                new ConsoleKeyInfo('t', ConsoleKey.T, false, false, false),
                new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false),
            });

            var parser = new ConsoleCommandLineParser();
            var result = parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);

            Assert.True(result.ThinkWeHaveSomething);
            Assert.Equal("Command1", result.Command.Name);
            Assert.Empty(result.FlaggedParameters);
            Assert.Empty(result.ValuedParameters);
            Assert.Equal("-t", result.RemainingText);
        }

        [Fact]
        public void WhenParsingTabbedInput_AssertTabResult()
        {
            var commandCache = GenerateCommandCache();

            _commandingConsoleSubstitute.SetConsoleKeyInfoOrder(new[]
            {
                new ConsoleKeyInfo('C', ConsoleKey.C, true, false, false),
                new ConsoleKeyInfo('o', ConsoleKey.O, false, false, false),
                new ConsoleKeyInfo('m', ConsoleKey.M, false, false, false),
                new ConsoleKeyInfo('m', ConsoleKey.M, false, false, false),
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),
                new ConsoleKeyInfo('n', ConsoleKey.N, false, false, false),
                new ConsoleKeyInfo('d', ConsoleKey.D, false, false, false),
                new ConsoleKeyInfo('1', ConsoleKey.NumPad1, false, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, false, false, false),

                new ConsoleKeyInfo('-', ConsoleKey.OemMinus, false, false, false),
                new ConsoleKeyInfo('t', ConsoleKey.T, false, false, false),
                new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false),
                new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false),
            });

            var parser = new ConsoleCommandLineParser();
            var result = parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);

            Assert.True(result.ThinkWeHaveSomething);
            Assert.Equal("Command1", result.Command.Name);
            Assert.Empty(result.FlaggedParameters);
            Assert.Equal("TestParam1", result.ValuedParameters.Single().Parameter.Name);
            Assert.Equal(string.Empty, result.RemainingText);
        }

        [Fact]
        public void WhenBackspacePressed_AssertTextRemoved()
        {
            var commandCache = GenerateCommandCache();

            _commandingConsoleSubstitute.SetConsoleKeyInfoOrder(new[]
            {
                new ConsoleKeyInfo('C', ConsoleKey.C, true, false, false),
                new ConsoleKeyInfo('o', ConsoleKey.O, false, false, false),
                new ConsoleKeyInfo('m', ConsoleKey.M, false, false, false),
                new ConsoleKeyInfo('m', ConsoleKey.M, false, false, false),
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),
                new ConsoleKeyInfo('n', ConsoleKey.N, false, false, false),
                new ConsoleKeyInfo('d', ConsoleKey.D, false, false, false),
                new ConsoleKeyInfo('1', ConsoleKey.NumPad1, false, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, false, false, false),

                new ConsoleKeyInfo('-', ConsoleKey.OemMinus, false, false, false),
                new ConsoleKeyInfo('t', ConsoleKey.T, false, false, false),
                new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false),
                new ConsoleKeyInfo('p', ConsoleKey.P, false, false, false),
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),
                new ConsoleKeyInfo('r', ConsoleKey.R, false, false, false),
                new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false),
                new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false),
            });

            var parser = new ConsoleCommandLineParser();
            var result = parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);

            Assert.True(result.ThinkWeHaveSomething);
            Assert.Equal("Command1", result.Command.Name);
            Assert.Empty(result.FlaggedParameters);
            Assert.Equal("Param1", result.ValuedParameters.Single().Parameter.Name);
            Assert.Equal(string.Empty, result.RemainingText);
        }

        [Fact]
        public void WhenParsingTabbedInputMultipleTimes_AssertTabResult()
        {
            var commandCache = GenerateCommandCache();

            _commandingConsoleSubstitute.SetConsoleKeyInfoOrder(new[]
            {
                new ConsoleKeyInfo('C', ConsoleKey.C, true, false, false),
                new ConsoleKeyInfo('o', ConsoleKey.O, false, false, false),
                new ConsoleKeyInfo('m', ConsoleKey.M, false, false, false),
                new ConsoleKeyInfo('m', ConsoleKey.M, false, false, false),
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),
                new ConsoleKeyInfo('n', ConsoleKey.N, false, false, false),
                new ConsoleKeyInfo('d', ConsoleKey.D, false, false, false),
                new ConsoleKeyInfo('1', ConsoleKey.NumPad1, false, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, false, false, false),

                new ConsoleKeyInfo('-', ConsoleKey.OemMinus, false, false, false),
                new ConsoleKeyInfo('p', ConsoleKey.T, false, false, false),
                new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false),
                new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false),
                new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false),

                new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false),
                new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false),
                new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false),

                new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false),
                new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false),
                new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false), // Tab a lot to make sure the rollover works

                new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, false, false, false),
                new ConsoleKeyInfo('v', ConsoleKey.V, false, false, false),
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),
                new ConsoleKeyInfo('l', ConsoleKey.L, false, false, false),
                new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false),
            });

            var parser = new ConsoleCommandLineParser();
            var result = parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);

            Assert.True(result.ThinkWeHaveSomething);
            Assert.Equal("Command1", result.Command.Name);
            Assert.Empty(result.FlaggedParameters);
            Assert.Equal("Param3", result.ValuedParameters.Single().Parameter.Name);
            Assert.Equal("val", result.ValuedParameters.Single().Value);
            Assert.Equal(string.Empty, result.RemainingText);
        }

        [Fact]
        public void WhenHittingTabButNothingToTabTo_AssertTextUnchanged()
        {
            var commandCache = GenerateCommandCache();

            _commandingConsoleSubstitute.SetConsoleKeyInfoOrder(new[]
            {
                new ConsoleKeyInfo('C', ConsoleKey.C, true, false, false),
                new ConsoleKeyInfo('o', ConsoleKey.O, false, false, false),
                new ConsoleKeyInfo('m', ConsoleKey.M, false, false, false),
                new ConsoleKeyInfo('m', ConsoleKey.M, false, false, false),
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),
                new ConsoleKeyInfo('n', ConsoleKey.N, false, false, false),
                new ConsoleKeyInfo('d', ConsoleKey.D, false, false, false),
                new ConsoleKeyInfo('1', ConsoleKey.NumPad1, false, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, false, false, false),

                new ConsoleKeyInfo('-', ConsoleKey.OemMinus, false, false, false),
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),
                new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false),
                new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false),

                new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, false, false, false),
                new ConsoleKeyInfo('v', ConsoleKey.V, false, false, false),
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),
                new ConsoleKeyInfo('l', ConsoleKey.L, false, false, false),
                new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false),
            });

            var parser = new ConsoleCommandLineParser();
            var result = parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);

            Assert.True(result.ThinkWeHaveSomething);
            Assert.Equal("Command1", result.Command.Name);
            Assert.Empty(result.FlaggedParameters);
            Assert.Equal("a", result.ValuedParameters.Single().Parameter.Name);
            Assert.Equal("val", result.ValuedParameters.Single().Value);
            Assert.Equal(string.Empty, result.RemainingText);
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
                new ParameterOption("TestParam1", true, "Parameter 3"),
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

using CommandComplete.ConsoleParsing;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;
using Shouldly;

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

            result.ThinkWeHaveSomething.ShouldBeTrue();
            result.Command.Name.ShouldBe("Command1");
            result.FlaggedParameters.ShouldBeEmpty();
            result.ValuedParameters.ShouldBeEmpty();
            result.RemainingText.ShouldBe("-t");
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

            result.ThinkWeHaveSomething.ShouldBeTrue();
            result.Command.Name.ShouldBe("Command1");
            result.FlaggedParameters.ShouldBeEmpty();
            result.ValuedParameters.Single().Parameter.Name.ShouldBe("TestParam1");
            result.RemainingText.ShouldBeEmpty();
        }

        [Fact]
        public void WhenCommandEnteredAndTabbingForParam_AssertParamShowed()
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

                new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false),
                new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false),
            });

            var parser = new ConsoleCommandLineParser();
            var result = parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);

            result.ThinkWeHaveSomething.ShouldBeTrue();
            result.Command.Name.ShouldBe("Command1");
            result.FlaggedParameters.ShouldBeEmpty();
            result.ValuedParameters.Single().Parameter.Name.ShouldBe("Param1");
            result.RemainingText.ShouldBeEmpty();
        }

        [Fact]
        public void WhenCommandEnteredWithNoParams_AssertCommandFound()
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
                new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false),
            });

            var parser = new ConsoleCommandLineParser();
            var result = parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);

            result.ThinkWeHaveSomething.ShouldBeTrue();
            result.Command.Name.ShouldBe("Command1");
            result.FlaggedParameters.ShouldBeEmpty();
            result.ValuedParameters.ShouldBeEmpty();
            result.RemainingText.ShouldBeEmpty();
        }

        [Fact]
        public void WhenTabbingWithNoCommandEntered_AssertCommandInserted()
        {
            var commandCache = GenerateCommandCache();

            _commandingConsoleSubstitute.SetConsoleKeyInfoOrder(new[]
            {
                new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false),
                new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false),
            });

            var parser = new ConsoleCommandLineParser();
            var result = parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);

            result.ThinkWeHaveSomething.ShouldBeTrue();
            result.Command.Name.ShouldBe("Command1");
            result.FlaggedParameters.ShouldBeEmpty();
            result.ValuedParameters.ShouldBeEmpty();
            result.RemainingText.ShouldBeEmpty();
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

            result.ThinkWeHaveSomething.ShouldBeTrue();
            result.Command.Name.ShouldBe("Command1");
            result.FlaggedParameters.ShouldBeEmpty();
            result.ValuedParameters.Single().Parameter.Name.ShouldBe("Param1");
            result.RemainingText.ShouldBeEmpty();
        }

        [Fact]
        public void WhenBackspacePressedManyTimes_AssertTextEmpty()
        {
            var commandCache = GenerateCommandCache();

            _commandingConsoleSubstitute.SetConsoleKeyInfoOrder(new[]
            {
                new ConsoleKeyInfo('C', ConsoleKey.C, true, false, false),
                new ConsoleKeyInfo('o', ConsoleKey.O, false, false, false),
                new ConsoleKeyInfo('m', ConsoleKey.M, false, false, false),
                new ConsoleKeyInfo('m', ConsoleKey.M, false, false, false),
                new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false),
                new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false),
                new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false),
                new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false),
                new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false),
                new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false),
                new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false),
                new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false),
                new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false),
                new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false),
                new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false),
                new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false),
            });

            var parser = new ConsoleCommandLineParser();
            var result = parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);

            result.RemainingText.ShouldBeEmpty();
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

            result.ThinkWeHaveSomething.ShouldBeTrue();
            result.Command.Name.ShouldBe("Command1");
            result.FlaggedParameters.ShouldBeEmpty();
            result.ValuedParameters.Single().Parameter.Name.ShouldBe("Param3");
            result.ValuedParameters.Single().Value.ShouldBe("val");
            result.RemainingText.ShouldBeEmpty();
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

            result.ThinkWeHaveSomething.ShouldBeTrue();
            result.Command.Name.ShouldBe("Command1");
            result.FlaggedParameters.ShouldBeEmpty();
            result.ValuedParameters.Single().Parameter.Name.ShouldBe("a");
            result.ValuedParameters.Single().Value.ShouldBe("val");
            result.RemainingText.ShouldBeEmpty();
        }

        [Fact]
        public void WhenHittingEscapeOnCommand_AssertCommandNameCleared()
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
                new ConsoleKeyInfo('\u0027', ConsoleKey.Escape, false, false, false),

                new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false),
            });

            var parser = new ConsoleCommandLineParser();
            var result = parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);

            result.Command.ShouldBeNull();
            result.FlaggedParameters.ShouldBeEmpty();
            result.ValuedParameters.ShouldBeEmpty();
            result.RemainingText.ShouldBeEmpty();
        }

        [Fact]
        public void WhenHittingEscapeOnParameterName_AssertParameterNameCleared()
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
                new ConsoleKeyInfo('b', ConsoleKey.B, false, false, false),
                new ConsoleKeyInfo('c', ConsoleKey.C, false, false, false),
                new ConsoleKeyInfo('\u0027', ConsoleKey.Escape, false, false, false),

                new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false),
            });

            var parser = new ConsoleCommandLineParser();
            var result = parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);

            result.Command.Name.ShouldBe("Command1");
            result.FlaggedParameters.ShouldBeEmpty();
            result.ValuedParameters.ShouldBeEmpty();
            result.RemainingText.ShouldBeEmpty();
        }

        [Fact]
        public void WhenHittingEscapeOnParameterValue_AssertParameterValueCleared()
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
                new ConsoleKeyInfo('p', ConsoleKey.A, false, false, false),
                new ConsoleKeyInfo('a', ConsoleKey.B, false, false, false),
                new ConsoleKeyInfo('r', ConsoleKey.C, false, false, false),
                new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, false, false, false),

                new ConsoleKeyInfo('d', ConsoleKey.D, false, false, false),
                new ConsoleKeyInfo('e', ConsoleKey.E, false, false, false),
                new ConsoleKeyInfo('f', ConsoleKey.F, false, false, false),
                new ConsoleKeyInfo('\u0027', ConsoleKey.Escape, false, false, false),

                new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false),
            });

            var parser = new ConsoleCommandLineParser();
            var result = parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);

            result.Command.Name.ShouldBe("Command1");
            result.FlaggedParameters.ShouldBeEmpty();
            result.ValuedParameters.Single().Parameter.Name.ShouldBe("Param1");
            result.ValuedParameters.Single().Value.ShouldBeEmpty();
            result.RemainingText.ShouldBeEmpty();
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

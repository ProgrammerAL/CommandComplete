using CommandComplete.ConsoleParsing;
using System;
using System.Linq;
using Xunit;
using Shouldly;

namespace CommandComplete.UnitTests.Console
{
    public class ConsoleCommandLineParserTest
    {
        private readonly CommandingConsoleFake _commandingConsoleSubstitute;

        public ConsoleCommandLineParserTest() => _commandingConsoleSubstitute = new CommandingConsoleFake();

        [Fact]
        public void WhenParsingInput_AssertDirectResult()
        {
            ICommandCache commandCache = GenerateCommandCache();

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
            ParseCommandLineResult result = parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);

            result.ThinkWeHaveSomething.ShouldBeTrue();
            result.Command.Name.ShouldBe("Command1");
            result.FlaggedParameters.ShouldBeEmpty();
            result.ValuedParameters.ShouldBeEmpty();
            result.RemainingText.ShouldBe("-t");
        }

        [Fact]
        public void WhenParsingTabbedInput_AssertTabResult()
        {
            ICommandCache commandCache = GenerateCommandCache();

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
            ParseCommandLineResult result = parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);

            result.ThinkWeHaveSomething.ShouldBeTrue();
            result.Command.Name.ShouldBe("Command1");
            result.FlaggedParameters.ShouldBeEmpty();
            result.ValuedParameters.Single().Parameter.Name.ShouldBe("TestParam1");
            result.RemainingText.ShouldBeEmpty();
        }

        [Fact]
        public void WhenCommandEnteredAndTabbingForParam_AssertParamShowed()
        {
            ICommandCache commandCache = GenerateCommandCache();

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
            ParseCommandLineResult result = parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);

            result.ThinkWeHaveSomething.ShouldBeTrue();
            result.Command.Name.ShouldBe("Command1");
            result.FlaggedParameters.ShouldBeEmpty();
            result.ValuedParameters.Single().Parameter.Name.ShouldBe("Param1");
            result.RemainingText.ShouldBeEmpty();
        }

        [Fact]
        public void WhenCommandEnteredWithNoParams_AssertCommandFound()
        {
            ICommandCache commandCache = GenerateCommandCache();

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
            ParseCommandLineResult result = parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);

            result.ThinkWeHaveSomething.ShouldBeTrue();
            result.Command.Name.ShouldBe("Command1");
            result.FlaggedParameters.ShouldBeEmpty();
            result.ValuedParameters.ShouldBeEmpty();
            result.RemainingText.ShouldBeEmpty();
        }

        [Fact]
        public void WhenTabbingWithNoCommandEntered_AssertCommandInserted()
        {
            ICommandCache commandCache = GenerateCommandCache();

            _commandingConsoleSubstitute.SetConsoleKeyInfoOrder(new[]
            {
                new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false),
                new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false),
            });

            var parser = new ConsoleCommandLineParser();
            ParseCommandLineResult result = parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);

            result.ThinkWeHaveSomething.ShouldBeTrue();
            result.Command.Name.ShouldBe("Command1");
            result.FlaggedParameters.ShouldBeEmpty();
            result.ValuedParameters.ShouldBeEmpty();
            result.RemainingText.ShouldBeEmpty();
        }

        [Fact]
        public void WhenBackspacePressed_AssertTextRemoved()
        {
            ICommandCache commandCache = GenerateCommandCache();

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
            ParseCommandLineResult result = parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);

            result.ThinkWeHaveSomething.ShouldBeTrue();
            result.Command.Name.ShouldBe("Command1");
            result.FlaggedParameters.ShouldBeEmpty();
            result.ValuedParameters.Single().Parameter.Name.ShouldBe("Param1");
            result.RemainingText.ShouldBeEmpty();
        }

        [Fact]
        public void WhenBackspacePressedManyTimes_AssertTextEmpty()
        {
            ICommandCache commandCache = GenerateCommandCache();

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
            ParseCommandLineResult result = parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);

            result.RemainingText.ShouldBeEmpty();
        }

        [Fact]
        public void WhenParsingTabbedInputMultipleTimes_AssertTabResult()
        {
            ICommandCache commandCache = GenerateCommandCache();

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
            ParseCommandLineResult result = parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);

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
            ICommandCache commandCache = GenerateCommandCache();

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
            ParseCommandLineResult result = parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);

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
            ICommandCache commandCache = GenerateCommandCache();

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
            ParseCommandLineResult result = parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);

            result.Command.ShouldBeNull();
            result.FlaggedParameters.ShouldBeEmpty();
            result.ValuedParameters.ShouldBeEmpty();
            result.RemainingText.ShouldBeEmpty();
        }

        [Fact]
        public void WhenHittingEscapeOnParameterName_AssertParameterNameCleared()
        {
            ICommandCache commandCache = GenerateCommandCache();

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
            ParseCommandLineResult result = parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);

            result.Command.Name.ShouldBe("Command1");
            result.FlaggedParameters.ShouldBeEmpty();
            result.ValuedParameters.ShouldBeEmpty();
            result.RemainingText.ShouldBeEmpty();
        }

        [Fact]
        public void WhenHittingEscapeOnParameterValue_AssertParameterValueCleared()
        {
            ICommandCache commandCache = GenerateCommandCache();

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
            ParseCommandLineResult result = parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);

            result.Command.Name.ShouldBe("Command1");
            result.FlaggedParameters.ShouldBeEmpty();
            result.ValuedParameters.Single().Parameter.Name.ShouldBe("Param1");
            result.ValuedParameters.Single().Value.ShouldBeEmpty();
            result.RemainingText.ShouldBeEmpty();
        }

        [Fact]
        public void WhenHittingEnterWithNothingEnteredIntoLine_AssertNotValid()
        {
            ICommandCache commandCache = GenerateCommandCache();

            _commandingConsoleSubstitute.SetConsoleKeyInfoOrder(new[]
            {
                new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false),
            });

            var parser = new ConsoleCommandLineParser();
            ParseCommandLineResult result = parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);

            result.ThinkWeHaveSomething.ShouldBeFalse();
        }

        [Fact]
        public void WhenUsingPastResultsUpArrow_AssertDirectResult()
        {
            ICommandCache commandCache = GenerateCommandCache();

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

            //Type in Command1 10 times, meaning the entire history is filled with this one
            var parser = new ConsoleCommandLineParser();
            for (int i = 0; i < 1; i++)
            {
                parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);
                _commandingConsoleSubstitute.ResetNextKeyIndex();
            }

            _commandingConsoleSubstitute.SetConsoleKeyInfoOrder(new[]
            {
                new ConsoleKeyInfo('C', ConsoleKey.C, true, false, false),
                new ConsoleKeyInfo('o', ConsoleKey.O, false, false, false),
                new ConsoleKeyInfo('m', ConsoleKey.M, false, false, false),
                new ConsoleKeyInfo('m', ConsoleKey.M, false, false, false),
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),
                new ConsoleKeyInfo('n', ConsoleKey.N, false, false, false),
                new ConsoleKeyInfo('d', ConsoleKey.D, false, false, false),
                new ConsoleKeyInfo('2', ConsoleKey.NumPad1, false, false, false),
                new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false),
            });

            //Type in Command2 9 times, so only the very last item is Command1
            for (int i = 0; i < 9; i++)
            {
                parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);
                _commandingConsoleSubstitute.ResetNextKeyIndex();
            }

            //Hit up arrow key 10 times. Meaning skip over the most recent Command2 and get Command1
            _commandingConsoleSubstitute.SetConsoleKeyInfoOrder(new[]
            {
                //Add a few misc characters to prove they're removed when using up arrow
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),

                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),
                new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false),
            });

            ParseCommandLineResult finalResult = parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);
            finalResult.ThinkWeHaveSomething.ShouldBeTrue();
            finalResult.Command.Name.ShouldBe("Command1");
        }

        [Fact]
        public void WhenUsingPastResultsDownArrow_AssertDirectResult()
        {
            ICommandCache commandCache = GenerateCommandCache();

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

            //Type in Command1 10 times, meaning the entire history is filled with this one
            var parser = new ConsoleCommandLineParser();
            for (int i = 0; i < 10; i++)
            {
                parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);
                _commandingConsoleSubstitute.ResetNextKeyIndex();
            }

            _commandingConsoleSubstitute.SetConsoleKeyInfoOrder(new[]
            {
                new ConsoleKeyInfo('C', ConsoleKey.C, true, false, false),
                new ConsoleKeyInfo('o', ConsoleKey.O, false, false, false),
                new ConsoleKeyInfo('m', ConsoleKey.M, false, false, false),
                new ConsoleKeyInfo('m', ConsoleKey.M, false, false, false),
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),
                new ConsoleKeyInfo('n', ConsoleKey.N, false, false, false),
                new ConsoleKeyInfo('d', ConsoleKey.D, false, false, false),
                new ConsoleKeyInfo('2', ConsoleKey.NumPad1, false, false, false),
                new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false),
            });

            //Type in Command2 3 times, so only the very last item is Command1
            for (int i = 0; i < 3; i++)
            {
                parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);
                _commandingConsoleSubstitute.ResetNextKeyIndex();
            }

            //Hit up down key 8 times. Meaning start looking at the end of the history buffer and end of on the last instance of Command2 typed in
            _commandingConsoleSubstitute.SetConsoleKeyInfoOrder(new[]
            {
                //Add a few misc characters to prove they're removed when using up arrow
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),

                new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, true, false, false),

                new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false),
            });

            ParseCommandLineResult finalResult = parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);
            finalResult.ThinkWeHaveSomething.ShouldBeTrue();
            finalResult.Command.Name.ShouldBe("Command2");
        }

        [Fact]
        public void WhenUsingUpArrowForPastResultsIsPressedALot_AssertDirectResult()
        {
            ICommandCache commandCache = GenerateCommandCache();

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

            //Type in Command1 10 times, meaning the entire history is filled with this one
            var parser = new ConsoleCommandLineParser();
            for (int i = 0; i < 10; i++)
            {
                parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);
                _commandingConsoleSubstitute.ResetNextKeyIndex();
            }

            _commandingConsoleSubstitute.SetConsoleKeyInfoOrder(new[]
            {
                new ConsoleKeyInfo('C', ConsoleKey.C, true, false, false),
                new ConsoleKeyInfo('o', ConsoleKey.O, false, false, false),
                new ConsoleKeyInfo('m', ConsoleKey.M, false, false, false),
                new ConsoleKeyInfo('m', ConsoleKey.M, false, false, false),
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),
                new ConsoleKeyInfo('n', ConsoleKey.N, false, false, false),
                new ConsoleKeyInfo('d', ConsoleKey.D, false, false, false),
                new ConsoleKeyInfo('2', ConsoleKey.NumPad1, false, false, false),
                new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false),
            });

            //Type in Command2 9 times, so only the very last item is Command1
            for (int i = 0; i < 9; i++)
            {
                parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);
                _commandingConsoleSubstitute.ResetNextKeyIndex();
            }

            //Hit up arrow key 9 times. Meaning skip over the most recent Command2 and get Command1
            _commandingConsoleSubstitute.SetConsoleKeyInfoOrder(new[]
            {
                //Add a few misc characters to prove they're removed when using up arrow
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),

                //Up arrow 10 times so it goes past the history buffer
                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),

                //Up arrow 10 more times to go to the only instance of Command1 in the history buffer
                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.UpArrow, true, false, false),
                new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false),
            });

            ParseCommandLineResult finalResult = parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);
            finalResult.ThinkWeHaveSomething.ShouldBeTrue();
            finalResult.Command.Name.ShouldBe("Command1");
        }

        [Fact]
        public void WhenUsingDownArrowForPastResultsIsPressedALot_AssertDirectResult()
        {
            ICommandCache commandCache = GenerateCommandCache();

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

            //Type in Command1 10 times, meaning the entire history is filled with this one
            var parser = new ConsoleCommandLineParser();
            for (int i = 0; i < 10; i++)
            {
                parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);
                _commandingConsoleSubstitute.ResetNextKeyIndex();
            }

            _commandingConsoleSubstitute.SetConsoleKeyInfoOrder(new[]
            {
                new ConsoleKeyInfo('C', ConsoleKey.C, true, false, false),
                new ConsoleKeyInfo('o', ConsoleKey.O, false, false, false),
                new ConsoleKeyInfo('m', ConsoleKey.M, false, false, false),
                new ConsoleKeyInfo('m', ConsoleKey.M, false, false, false),
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),
                new ConsoleKeyInfo('n', ConsoleKey.N, false, false, false),
                new ConsoleKeyInfo('d', ConsoleKey.D, false, false, false),
                new ConsoleKeyInfo('2', ConsoleKey.NumPad1, false, false, false),
                new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false),
            });

            //Type in Command2 one time, so only the first command in history buffer is Command2 and the rest are Command1
            for (int i = 0; i < 1; i++)
            {
                parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);
                _commandingConsoleSubstitute.ResetNextKeyIndex();
            }

            //Hit up arrow key 9 times. Meaning skip over the most recent Command2 and get Command1
            _commandingConsoleSubstitute.SetConsoleKeyInfoOrder(new[]
            {
                //Add a few misc characters to prove they're removed when using up arrow
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),
                new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false),

                //Down arrow 10 times so it goes past the history buffer
                new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, true, false, false),

                //Down arrow 10 more times to go to the only instance of Command2 in the history buffer. The one at the front. Index 0
                new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, true, false, false),
                new ConsoleKeyInfo(' ', ConsoleKey.DownArrow, true, false, false),
                new ConsoleKeyInfo('\n', ConsoleKey.Enter, false, false, false),
            });

            ParseCommandLineResult finalResult = parser.ParseCommandLine(commandCache, _commandingConsoleSubstitute);
            finalResult.ThinkWeHaveSomething.ShouldBeTrue();
            finalResult.Command.Name.ShouldBe("Command2");
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

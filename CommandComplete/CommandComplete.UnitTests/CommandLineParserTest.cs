using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;
using CommandComplete;
using Shouldly;

namespace CommandComplete.UnitTests
{
    public class CommandLineParserTest
    {
        [Fact]
        public void WhenParsingCommandWithParams_AssertResult()
        {
            var commandCache = GenerateCommandCache();
            var parser = new CommandLineParser();
            var result = parser.ParseCommandLine("command1 -flagParam1 -param1 value1 -para", commandCache);

            result.ThinkWeHaveSomething.ShouldBeTrue();
            result.Command.Name.ShouldBe("command1", StringCompareShould.IgnoreCase);
            result.FlaggedParameters.Single().Name.ShouldBe("FlagParam1");
            result.ValuedParameters.Single().Parameter.Name.ShouldBe("Param1");
            result.RemainingText.ShouldBe("-para");
            result.PossibleTextsToAutofill.Count.ShouldBe(2);
        }

        [Fact]
        public void WhenParameterDoesNotHaveValue_AssertParameterFoundWithNoValue()
        {
            var commandCache = GenerateCommandCache();
            var parser = new CommandLineParser();
            var result = parser.ParseCommandLine("command1 -param1", commandCache);

            result.ThinkWeHaveSomething.ShouldBe(true);
            result.Command.Name.ShouldBe("command1", StringCompareShould.IgnoreCase);
            result.ValuedParameters.Single().Parameter.Name.ShouldBe("Param1");
        }

        [Fact]
        public void WhenParameterValueHasQuotes_AssertSpaceIncludedInParamValue()
        {
            var commandCache = GenerateCommandCache();
            var parser = new CommandLineParser();
            var result = parser.ParseCommandLine("command1 -param1 \"Some Value I Made up\"", commandCache);

            result.ThinkWeHaveSomething.ShouldBe(true);
            result.Command.Name.ShouldBe("command1", StringCompareShould.IgnoreCase);
            result.ValuedParameters.Single().Parameter.Name.ShouldBe("Param1");
            result.ValuedParameters.Single().Value.ShouldBe("Some Value I Made up");
        }

        [Fact]
        public void WhenParameterValueHasQuotesButNotAtEnd_AssertFullTextIncluded()
        {
            var commandCache = GenerateCommandCache();
            var parser = new CommandLineParser();
            var result = parser.ParseCommandLine("command1 -param1 \"Some Value without end quotes", commandCache);

            result.ThinkWeHaveSomething.ShouldBe(true);
            result.Command.Name.ShouldBe("command1", StringCompareShould.IgnoreCase);
            result.ValuedParameters.Single().Parameter.Name.ShouldBe("Param1");
            result.ValuedParameters.Single().Value.ShouldBe("Some Value without end quotes");
        }

        [Fact]
        public void WhenEnteredCommandName_AssertCommandOptionsFound()
        {
            var commandCache = GenerateCommandCache();
            var parser = new CommandLineParser();
            var result = parser.ParseCommandLine("comman", commandCache);

            result.ThinkWeHaveSomething.ShouldBeFalse();
            result.PossibleTextsToAutofill.Count.ShouldBe(2);
        }

        [Fact]
        public void WhenEnteredCommandNameWithNoSpace_AssertCommandFound()
        {
            var commandCache = GenerateCommandCache();
            var parser = new CommandLineParser();
            var result = parser.ParseCommandLine("Command1", commandCache);

            result.ThinkWeHaveSomething.ShouldBeTrue();
            result.Command.Name.ShouldBe("Command1");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void WhenTextEmpty_AssertResult(string commandText)
        {
            var commandCache = GenerateCommandCache();
            var parser = new CommandLineParser();
            var result = parser.ParseCommandLine(commandText, commandCache);

            result.ThinkWeHaveSomething.ShouldBeFalse();
        }

        [Fact]
        public void WhenCommandEnteredButNotInCache_AssertResult()
        {
            var commandCache = GenerateCommandCache();
            var parser = new CommandLineParser();
            var result = parser.ParseCommandLine("jshdb ", commandCache);

            result.ShouldBe(ParseCommandLineResult.CouldNotParseCommand);
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

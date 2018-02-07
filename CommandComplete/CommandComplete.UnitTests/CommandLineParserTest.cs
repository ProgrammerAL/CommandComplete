using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;
using CommandComplete;
using System.Diagnostics.CodeAnalysis;

namespace CommandComplete.UnitTests
{
    [ExcludeFromCodeCoverage]
    public class CommandLineParserTest
    {
        [Fact]
        public void WhenParsingCommandWithParams_AssertResult()
        {
            var commandCache = GenerateCommandCache();
            var parser = new CommandLineParser();
            var result = parser.ParseCommandLine("command1 -flagParam1 -param1 value1 -para", commandCache);

            Assert.True(result.ThinkWeHaveSomething);
            Assert.Equal("command1", result.Command.Name, ignoreCase: true);
            Assert.Equal("FlagParam1", result.FlaggedParameters.Single().Name);
            Assert.Equal("Param1", result.ValuedParameters.Single().Parameter.Name);

            Assert.Equal("-para", result.RemainingText);

            Assert.Equal(2, result.PossibleTextsToAutofill.Count);
        }

        [Fact]
        public void WhenParameterDoesNotHaveValue_AssertParameterFoundWithNoValue()
        {
            var commandCache = GenerateCommandCache();
            var parser = new CommandLineParser();
            var result = parser.ParseCommandLine("command1 -param1", commandCache);

            Assert.True(result.ThinkWeHaveSomething);
            Assert.Equal("command1", result.Command.Name, ignoreCase: true);
            Assert.Equal("Param1", result.ValuedParameters.Single().Parameter.Name);
        }

        [Fact]
        public void WhenParameterValueHasQuotes_AssertSpaceIncludedInParamValue()
        {
            var commandCache = GenerateCommandCache();
            var parser = new CommandLineParser();
            var result = parser.ParseCommandLine("command1 -param1 \"Some Value I Made up\"", commandCache);

            Assert.True(result.ThinkWeHaveSomething);
            Assert.Equal("command1", result.Command.Name, ignoreCase: true);
            Assert.Equal("Param1", result.ValuedParameters.Single().Parameter.Name);
            Assert.Equal("Some Value I Made up", result.ValuedParameters.Single().Value);
        }
        
        [Fact]
        public void WhenParameterValueHasQuotesButNotAtEnd_AssertFullTextIncluded()
        {
            var commandCache = GenerateCommandCache();
            var parser = new CommandLineParser();
            var result = parser.ParseCommandLine("command1 -param1 \"Some Value without end quotes", commandCache);

            Assert.True(result.ThinkWeHaveSomething);
            Assert.Equal("command1", result.Command.Name, ignoreCase: true);
            Assert.Equal("Param1", result.ValuedParameters.Single().Parameter.Name);
            Assert.Equal("Some Value without end quotes", result.ValuedParameters.Single().Value);
        }

        [Fact]
        public void WhenEnteredCommandName_AssertCommandOptionsFound()
        {
            var commandCache = GenerateCommandCache();
            var parser = new CommandLineParser();
            var result = parser.ParseCommandLine("comman", commandCache);

            Assert.True(result.ThinkWeHaveSomething);
            Assert.Equal(2, result.PossibleTextsToAutofill.Count);
        }
        
        [Fact]
        public void WhenEnteredCommandNameWithNoSpace_AssertCommandFound()
        {
            var commandCache = GenerateCommandCache();
            var parser = new CommandLineParser();
            var result = parser.ParseCommandLine("Command1", commandCache);

            Assert.True(result.ThinkWeHaveSomething);
            Assert.Equal("Command1", result.Command.Name);
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

            Assert.True(result.ThinkWeHaveSomething);
        }

        [Fact]
        public void WhenCommandEnteredButNotInCache_AssertResult()
        {
            var commandCache = GenerateCommandCache();
            var parser = new CommandLineParser();
            var result = parser.ParseCommandLine("jshdb ", commandCache);

            Assert.Equal(ParseCommandLineResult.CouldNotParseCommand, result);
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

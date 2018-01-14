using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace CommandComplete
{
    public class ParseCommandLineResult
    {
        public static readonly ParseCommandLineResult CouldNotParseCommand = new ParseCommandLineResult();

        public static ParseCommandLineResult GenerateWithOneHigherTabbedCount(ParseCommandLineResult previousResult)
        {
            return new ParseCommandLineResult(
                previousResult.Command,
                previousResult.FlaggedParameters,
                previousResult.ValuedParameters,
                previousResult.RemainingText,
                previousResult.PossibleTextsToAutofill,
                previousResult.TabbedCount + 1
                );
        }

        public ParseCommandLineResult()
        {
            ThinkWeHaveSomething = false;
            Command = null;
            FlaggedParameters = ImmutableList.Create<ParameterOption>();
            ValuedParameters = ImmutableList.Create<(ParameterOption, string)>();
        }

        /// <param name="command">The name of the command being used</param>
        /// <param name="flaggedParameters">Collection of flag parameters (parameters without a value)</param>
        /// <param name="valuedParameters">Collection of parameters that have a key/value pair</param>
        /// <param name="remainingText">Text at the end of the command line that has not been parsed</param>
        /// <param name="possibleTextsToAutofill">Possible sets of text that can be added to the end of the command string</param>
        /// <param name="tabbedCount">How many times the Tab key has been used on this set of options</param>
        public ParseCommandLineResult(Command command,
            IEnumerable<ParameterOption> flaggedParameters,
            IEnumerable<(ParameterOption, string)> valuedParameters,
            string remainingText,
            IEnumerable<ParameterOption> possibleTextsToAutofill,
            int tabbedCount)
        {
            ThinkWeHaveSomething = true;

            Command = command;
            FlaggedParameters = flaggedParameters?.ToImmutableList() ?? ImmutableList.Create<ParameterOption>();
            ValuedParameters = valuedParameters?.ToImmutableList() ?? ImmutableList.Create<(ParameterOption, string)>();

            RemainingText = remainingText;
            PossibleTextsToAutofill = possibleTextsToAutofill?.ToImmutableList() ?? ImmutableList.Create<ParameterOption>();

            TabbedCount = tabbedCount;
        }

        /// <summary>
        /// True if ???
        /// </summary>
        public bool ThinkWeHaveSomething { get; }

        /// <summary>
        /// The name of the command being used
        /// </summary>
        public Command Command { get; }

        /// <summary>
        /// Collection of flag parameters (parameters without a value)
        /// </summary>
        public IImmutableList<ParameterOption> FlaggedParameters { get; }

        /// <summary>
        /// Collection of parameters that have a key/value pair
        /// </summary>
        public IImmutableList<(ParameterOption Parameter, string Value)> ValuedParameters { get; }

        /// <summary>
        /// Text at the end of the command line that has not been parsed
        /// </summary>
        public string RemainingText { get; }

        /// <summary>
        /// Possible sets of text that can be added to the end of the command string
        /// </summary>
        public IImmutableList<ParameterOption> PossibleTextsToAutofill { get; }

        /// <summary>
        /// How many times the Tab key has been used on this set of options
        /// </summary>
        public int TabbedCount { get; }
    }
}

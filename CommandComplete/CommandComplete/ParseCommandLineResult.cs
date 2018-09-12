using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace CommandComplete
{
    public class ParseCommandLineResult
    {
        public static readonly ParseCommandLineResult CouldNotParseCommand = new ParseCommandLineResult();

        public ParseCommandLineResult()
        {
            ThinkWeHaveSomething = false;
            Command = null;
            FlaggedParameters = ImmutableList.Create<ParameterOption>();
            ValuedParameters = ImmutableList.Create<(ParameterOption, string)>();
            RemainingText = string.Empty;
            PossibleTextsToAutofill = ImmutableList.Create<string>();
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
            IEnumerable<string> possibleTextsToAutofill
            )
        {
            ThinkWeHaveSomething = command != null;

            Command = command;
            FlaggedParameters = flaggedParameters?.ToImmutableList() ?? ImmutableList.Create<ParameterOption>();
            ValuedParameters = valuedParameters?.ToImmutableList() ?? ImmutableList.Create<(ParameterOption, string)>();

            RemainingText = remainingText ?? string.Empty;
            PossibleTextsToAutofill = possibleTextsToAutofill?.ToImmutableList() ?? ImmutableList.Create<string>();
        }

        /// <summary>
        /// True if Command not null
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
        public IImmutableList<string> PossibleTextsToAutofill { get; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(Command?.Name ?? string.Empty);

            foreach (var flaggedParam in FlaggedParameters)
            {
                builder.Append(" " + Command.ParameterHeader +  flaggedParam.Name);
            }

            foreach (var valuedParam in ValuedParameters)
            {
                builder.Append(" " + Command.ParameterHeader + valuedParam.Parameter.Name + " " + valuedParam.Value);
            }

            builder.Append(RemainingText);
            return builder.ToString();
        }
    }
}

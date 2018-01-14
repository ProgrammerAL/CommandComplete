using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Linq;

namespace CommandComplete
{
    public class Command
    {
        /// <summary>
        /// Contains all characters that can't be used as part of a command's name. 
        /// Note: There are probably a lot more that should go here I guess. Will be added at a later time.
        /// </summary>
        public static readonly IImmutableList<char> InvalidCharactersForCommandName =
            new[] { ' ', '-', '/', '\\', '\b', '\t', }.ToImmutableList();

        /// <param name="name"></param>
        /// <param name="parameterHeader">The character at the beginning of the parameter name to mark it as a parameter. </param>
        /// <param name="helpText"></param>
        /// <param name="parameters"></param>
        public Command(string name, char parameterHeader, string helpText, IEnumerable<ParameterOption> parameters)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"Input parameter {nameof(name)} is null, empty, or whitespace");
            }
            else if (name.Any(x => InvalidCharactersForCommandName.Contains(x)))
            {
                throw new ArgumentException($"Input parameter {nameof(name)} contains an invalid character from the list: {new string(InvalidCharactersForCommandName.ToArray())}");
            }

            Name = name;
            ParameterHeader = parameterHeader;
            HelpText = helpText ?? string.Empty;
            Parameters = parameters?.ToImmutableList() ?? ImmutableList.Create<ParameterOption>();
        }

        /// <summary>
        /// The character at the beginning of the parameter name to mark it as a parameter. 
        /// Ex: '-' for -param, or '/' for /param
        /// </summary>
        public char ParameterHeader { get; }

        public string Name { get; }
        public IImmutableList<ParameterOption> Parameters { get; }

        /// <summary>
        /// Human Readable text displayed to help user know what this command does
        /// </summary>
        public string HelpText { get; set; }

        public bool TryGetParameterWithNameIgnoringHeader(string name, out ParameterOption matchingParamerter)
        {
            var paramName = name.TrimStart(ParameterHeader);
            matchingParamerter = Parameters.FirstOrDefault(x => string.Equals(x.Name, paramName, StringComparison.OrdinalIgnoreCase));
            return matchingParamerter != null;
        }

        /// <summary>
        /// When implemented, will ???
        /// But it's not implemented yet. So don't use it. Or use it, but comment out the calling line. 
        /// You're smart, I'm sure you'll know what to do.
        /// </summary>
        public string GenerateHelpString()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the collection of parameters that might be able to be used with the remaining set of text
        /// </summary>
        public IList<ParameterOption> GetPossibleParametersThatStartWith(string remainingText, IImmutableList<ParameterOption> parametersToExclude)
        {
            var remaingTextWithoutHeader = remainingText;
            if (remaingTextWithoutHeader.StartsWith(ParameterHeader.ToString()))
            {
                remaingTextWithoutHeader = remaingTextWithoutHeader.Substring(1);
            }

            var parametersWithoutExclusions = Parameters.Except(parametersToExclude);

            if (string.IsNullOrWhiteSpace(remaingTextWithoutHeader))
            {
                return parametersWithoutExclusions.ToList();
            }
            else
            {
                return parametersWithoutExclusions
                        .Where(x => x.Name.StartsWith(remaingTextWithoutHeader, StringComparison.OrdinalIgnoreCase))
                        .ToList();
            }
        }
    }
}

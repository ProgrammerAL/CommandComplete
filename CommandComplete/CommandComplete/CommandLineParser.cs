using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.Immutable;

namespace CommandComplete
{
    public class CommandLineParser
    {
        private const char CommandLineSeparatorChar_Space = ' ';

        public ParseCommandLineResult ParseCommandLine(string textSoFar, ParseCommandLineResult previousParseResult, ICommandCache commandsCache)
        {
            var parseResultFromThisRunThrough = previousParseResult ?? ParseCommandLineResult.CouldNotParseCommand;

            //Steps:
            // - Check if command name has been set yet by searching string for a space
            // - If no space, try to find a command that matches the already existing name
            // - If has space, try to find command and then try to use parameter
            parseResultFromThisRunThrough = ParseCommandNameFromText(textSoFar, commandsCache);

            return parseResultFromThisRunThrough;
        }

        private ParseCommandLineResult ParseCommandNameFromText(string userTextSoFar, ICommandCache commandsCache)
        {
            var sanitizedText = SanitizeUserText(userTextSoFar);
            var spaceIndex = sanitizedText.IndexOf(CommandLineSeparatorChar_Space);
            if (!SpaceCharacterFound(spaceIndex))
            {
                //Don't have a command set yet, so try to find one that matches 
                var possibleCommandNames = commandsCache.PossibleCommands
                                                    .Where(x => x.Name.StartsWith(sanitizedText, StringComparison.OrdinalIgnoreCase))
                                                    .Select(x => x.Name);
                return new ParseCommandLineResult(null, null, null, sanitizedText, possibleCommandNames);
            }
            else
            {
                //Guess we know the command
                var commandName = sanitizedText.Substring(0, spaceIndex);
                var knownCommand = commandsCache
                                        .PossibleCommands
                                        .FirstOrDefault(x => string.Equals(x.Name, commandName, StringComparison.OrdinalIgnoreCase));

                if (knownCommand == null)
                {
                    //Didn't find the command
                    return ParseCommandLineResult.CouldNotParseCommand;
                }
                else
                {
                    var textAfterCommand = sanitizedText.Substring(spaceIndex + 1);
                    var parameters = textAfterCommand.Split(CommandLineSeparatorChar_Space);

                    IList<(ParameterOption Param, string Value)> valuedParameters = new List<(ParameterOption Param, string Value)>();
                    IList<ParameterOption> flaggedParameters = new List<ParameterOption>();
                    IList<string> possibleNextParameters = new List<string>();
                    string remainingText = null;

                    if (parameters.Any())
                    {
                        int i = 0;
                        while (i < parameters.Length)
                        {
                            bool grabbedParamValue = false;
                            var paramName = parameters[i];
                            if (knownCommand.TryGetParameterWithNameIgnoringHeader(paramName, out ParameterOption knownParam))
                            {
                                //Then we already know about this param
                                if (knownParam.TakesInputValue)
                                {
                                    if (ParametersHasValueForParamAtIndex(parameters, i))
                                    {
                                        var paramValue = parameters[i + 1];
                                        valuedParameters.Add((knownParam, paramValue));
                                        grabbedParamValue = true;
                                    }
                                }
                                else
                                {
                                    flaggedParameters.Add(knownParam);
                                }
                            }
                            else
                            {
                                //We don't know about the parameter with the name.
                                //  Maybe someone forgot about it in the list, or user hasn't typed in the full text because it at the end.
                                if (IndexIsForLastParameter(i, parameters))
                                {
                                    //If we're on the last param value user is probably mid-way through typing it
                                    remainingText = paramName;
                                    var parametersAlreadyUsed = valuedParameters.Select(x => x.Param).Concat(flaggedParameters).ToImmutableList();
                                    possibleNextParameters = knownCommand.GetPossibleParametersThatStartWith(remainingText, parametersAlreadyUsed).Select(x => x.Name).ToList();
                                }
                            }

                            if (grabbedParamValue)
                            {
                                i += 2;
                            }
                            else
                            {
                                i++;
                            }
                        }
                    }

                    return new ParseCommandLineResult(knownCommand,
                        flaggedParameters,
                        valuedParameters,
                        remainingText,
                        possibleNextParameters);
                }
            }
        }

        private string SanitizeUserText(string userText)
        {
            return userText?.TrimStart() ?? string.Empty;
        }

        private bool SpaceCharacterFound(int spaceIndex)
        {
            return spaceIndex > -1;
        }

        private bool ParametersHasValueForParamAtIndex(string[] parameters, int index)
        {
            return (parameters.Length >= index + 1);
        }

        private bool IndexIsForLastParameter(int i, string[] parameters)
        {
            return i == (parameters.Length - 1);
        }
    }
}

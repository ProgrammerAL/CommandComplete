using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.Immutable;

namespace CommandComplete
{
    public class CommandLineParser
    {
        private const char SpaceChar = ' ';
        private const string DoubleQuote = "\"";
        private const string EscapedDoubleQuote = "\\\"";

        public ParseCommandLineResult ParseCommandLine(string userTextSoFar, ICommandCache commandsCache)
        {
            //Steps:
            // - Check if command name has been set yet by searching string for a space
            // - If no space, try to find a command that matches the already existing name
            // - If has space, try to find command and then try to use parameter

            var sanitizedText = SanitizeUserText(userTextSoFar);
            var spaceIndex = sanitizedText.IndexOf(SpaceChar);
            if (!SpaceCharacterFound(spaceIndex))
            {
                //Don't have a command set yet, so try to find one that matches 
                var possibleCommandNames = commandsCache.PossibleCommands
                                                    .Where(x => x.Name.StartsWith(sanitizedText, StringComparison.OrdinalIgnoreCase))
                                                    .Select(x => x.Name)
                                                    .OrderBy(x => x);

                var existingCommand = commandsCache.PossibleCommands
                                                   .FirstOrDefault(command => command.Name.Equals(sanitizedText, StringComparison.OrdinalIgnoreCase));

                var remainingText = sanitizedText;
                if (existingCommand != null)
                {
                    remainingText = string.Empty;
                }

                return new ParseCommandLineResult(existingCommand, null, null, remainingText, possibleCommandNames);
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
                    var parameters = SplitParameters(textAfterCommand, SpaceChar);

                    IList<(ParameterOption Param, string Value)> valuedParameters = new List<(ParameterOption Param, string Value)>();
                    IList<ParameterOption> flaggedParameters = new List<ParameterOption>();
                    IList<string> possibleNextParameters = new List<string>();
                    string remainingText = string.Empty;

                    if (parameters.Any())
                    {
                        int i = 0;
                        while (i < parameters.Count)
                        {
                            bool grabbedParamValue = false;
                            var paramName = parameters[i];
                            if (knownCommand.TryGetParameterWithNameIgnoringHeader(paramName, out ParameterOption knownParam))
                            {
                                //Then we already know about this param
                                if (knownParam.TakesInputValue)
                                {
                                    grabbedParamValue = PullOutValuedParameter(parameters, valuedParameters, i, grabbedParamValue, knownParam);
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
                                else
                                {
                                    //Since it's not last, this is a parameter we don't know about
                                    //  Default to assuming it takes a value and isn't a flag
                                    paramName = paramName.TrimStart(knownCommand.ParameterHeader);
                                    knownParam = new ParameterOption(paramName, true, "Unknown Parameter");

                                    grabbedParamValue = PullOutValuedParameter(parameters, valuedParameters, i, grabbedParamValue, knownParam);
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

        private IImmutableList<string> SplitParameters(string textAfterCommand, char commandLineSeparatorChar_Space)
        {
            var spaceSplitParameters = textAfterCommand.Split(SpaceChar);
            var paramList = new List<string>(spaceSplitParameters.Length);

            bool itemInQuotes = false;
            var textInQuotes = new StringBuilder();

            foreach (var paramText in spaceSplitParameters)
            {
                if (itemInQuotes)
                {
                    textInQuotes.Append(SpaceChar);
                    textInQuotes.Append(paramText);

                    var itemLeftQuotes = StringEndsWithUnescapedDoubleQuote(paramText);
                    if(itemLeftQuotes)
                    {
                        var textWithoutDoubleQuote = GetTextWithoutDoubleQuoteCharAtEnd(textInQuotes.ToString());
                        paramList.Add(textWithoutDoubleQuote);
                        itemInQuotes = false;
                        textInQuotes.Clear();
                    }
                }
                else
                {
                    itemInQuotes = StringStartsWithUnescapedDoubleQuote(paramText);

                    if (itemInQuotes)
                    {
                        var textWithoutDoubleQuote = paramText.Substring(1);
                        textInQuotes.Clear();
                        textInQuotes.Append(textWithoutDoubleQuote);
                    }
                    else
                    {
                        paramList.Add(paramText);
                    }
                }
            }

            //Double quotes are missing from end of string. I guess include this at the end
            if (textInQuotes.Length > 0)
            {
                paramList.Add(textInQuotes.ToString());
            }

            return paramList.ToImmutableList();
        }

        private string GetTextWithoutDoubleQuoteCharAtEnd(string paramText)
        {
            var lengthToLastCharNotCountingDoubleQuote = paramText.Length - 1;
            return paramText.Substring(0, lengthToLastCharNotCountingDoubleQuote);
        }

        private bool StringStartsWithUnescapedDoubleQuote(string paramText)
        {
            return paramText.StartsWith(DoubleQuote, StringComparison.OrdinalIgnoreCase);
        }

        private bool StringEndsWithUnescapedDoubleQuote(string paramText)
        {
            return paramText.EndsWith(DoubleQuote, StringComparison.OrdinalIgnoreCase)
                && !paramText.EndsWith(EscapedDoubleQuote, StringComparison.OrdinalIgnoreCase);
        }

        private bool PullOutValuedParameter(IImmutableList<string> parameters, IList<(ParameterOption Param, string Value)> valuedParameters, int index, bool grabbedParamValue, ParameterOption knownParam)
        {
            if (ParametersHasValueForParamAtIndex(parameters, index))
            {
                var paramValue = parameters[index + 1];
                valuedParameters.Add((knownParam, paramValue));
                grabbedParamValue = true;
            }
            else
            {
                valuedParameters.Add((knownParam, string.Empty));
            }

            return grabbedParamValue;
        }

        private string SanitizeUserText(string userText)
        {
            return userText?.TrimStart() ?? string.Empty;
        }

        private bool SpaceCharacterFound(int spaceIndex)
        {
            return spaceIndex > -1;
        }

        private bool ParametersHasValueForParamAtIndex(IImmutableList<string> parameters, int index)
        {
            int lastIndexOfParameters = (parameters.Count - 1);
            var indexOfParameterToLookFor = (index + 1);

            return lastIndexOfParameters >= indexOfParameterToLookFor;
        }

        private bool IndexIsForLastParameter(int i, IImmutableList<string> parameters)
        {
            return i == (parameters.Count - 1);
        }
    }
}

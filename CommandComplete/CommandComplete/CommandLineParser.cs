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

        public ParseCommandLineResult ParseCommandLine(string textSoFar, ConsoleKey mostRecentKey, ParseCommandLineResult previousParseResult, ICommandCache commandsCache)
        {
            var parseResultFromThisRunThrough = previousParseResult ?? ParseCommandLineResult.CouldNotParseCommand;

            if (mostRecentKey == ConsoleKey.Tab)
            {
                //On Tab pressed, try to do auto-complete for next set of text
                if (parseResultFromThisRunThrough != null
                    && parseResultFromThisRunThrough.ThinkWeHaveSomething)
                {
                    parseResultFromThisRunThrough = ParseCommandLineResult.GenerateWithOneHigherTabbedCount(parseResultFromThisRunThrough);
                }
            }
            else
            {
                //Steps:
                // - Check if command name has been set yet by searching string for a space
                // - If no space, try to find a command that matches the already existing name
                // - If has space, try to find command and then try to use parameter
                //var userTextSoFar = builder.ToString();
                parseResultFromThisRunThrough = ParseCommandNameFromText(textSoFar, commandsCache);
            }

            return parseResultFromThisRunThrough;
        }

        private ParseCommandLineResult ParseCommandNameFromText(string userTextSoFar, ICommandCache commandsCache)
        {
            var spaceIndex = userTextSoFar.IndexOf(CommandLineSeparatorChar_Space);
            if (spaceIndex == -1)
            {
                //Don't have a command set yet, so try to find one that matches 

            }
            else
            {
                //Guess we know the command
                var commandName = userTextSoFar.Substring(0, spaceIndex);
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
                    var textAfterCommand = userTextSoFar.Substring(spaceIndex + 1);
                    var parameters = textAfterCommand.Split(CommandLineSeparatorChar_Space);

                    IList<(ParameterOption Param, string Value)> valuedParameters = new List<(ParameterOption Param, string Value)>();
                    IList<ParameterOption> flaggedParameters = new List<ParameterOption>();
                    IList<ParameterOption> possibleNextParameters = new List<ParameterOption>();
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
                                    possibleNextParameters = knownCommand.GetPossibleParametersThatStartWith(remainingText, parametersAlreadyUsed);
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
                        possibleNextParameters,
                        0);
                }
            }

            //TODO: Finish this method. Return line below is temporary
            return ParseCommandLineResult.CouldNotParseCommand;
        }

        private bool ParametersHasValueForParamAtIndex(string[] parameters, int index)
        {
            return (parameters.Length >= index + 1);
        }

        private bool IndexIsForLastParameter(int i, string[] parameters)
        {
            return i == (parameters.Length - 1);
        }

        private static bool CanChooseNextCommandInList(IList<string> commandsToChooseFrom, int indexOfLastCommandGuessed)
        {
            return indexOfLastCommandGuessed > -1 && indexOfLastCommandGuessed + 1 < commandsToChooseFrom.Count;
        }

        //private bool KeyIsCommandAcceptable(ConsoleKeyInfo nextKey, ICommandCache commandsCache)
        //{
        //    return char.IsLetterOrDigit(nextKey.KeyChar)
        //        || nextKey.Key == ConsoleKey.Spacebar
        //        || nextKey.KeyChar == commandsCache.ParameterHeader;
        //}
    }
}

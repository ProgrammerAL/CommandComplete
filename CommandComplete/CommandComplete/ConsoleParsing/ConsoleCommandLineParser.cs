using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CommandComplete.ConsoleParsing
{
    public class ConsoleCommandLineParser
    {
        public ParseCommandLineResult ParseCommandLine(ICommandCache commandCache, ICommandingConsole console)
        {
            var builder = new StringBuilder();
            var previousParseResult = ParseCommandLineResult.CouldNotParseCommand;
            int tabbedCount = 0;

            var parser = new CommandLineParser();

            bool isParsingFromCommandLine = true;
            while (isParsingFromCommandLine)
            {
                var nextKey = console.GetGetKey(true);
                if (nextKey.Key == ConsoleKey.Enter)
                {
                    console.Write(Environment.NewLine);
                    isParsingFromCommandLine = false;
                }
                else if (nextKey.Key == ConsoleKey.Backspace)
                {
                    TrimEndCharacters(builder, console, 1);
                }
                else if (nextKey.Key == ConsoleKey.Tab)
                {
                    if (previousParseResult.ThinkWeHaveSomething)
                    {
                        int charactersToRemove = previousParseResult.RemainingText.Length;

                        if (tabbedCount > 0)
                        {
                            //User hit tab before this instance, so remove the text they previously entered before adding something else
                            var previousTextTabbedIntoConsole = GetTextToAddToConsoleAtTabCount(previousParseResult, tabbedCount - 1);
                            charactersToRemove = previousTextTabbedIntoConsole.Length;
                        }

                        //else if (previousParseResult.Command != null)
                        //{
                        //    //This means we're hitting tab for a parameter, not a command
                        //    // But we only check this the first time the Tab is pressed. 
                        //    //      Because we don't want to remove the Header character
                        //    //  So check for the Header character, like a -
                        //    charactersToRemove--;
                        //}

                        var textToAppend = GetTextToAddToConsoleAtTabCount(previousParseResult, tabbedCount);
                        if (!string.IsNullOrEmpty(textToAppend))
                        {
                            TrimEndCharacters(builder, console, charactersToRemove);
                            AppendText(builder, console, textToAppend);
                            tabbedCount++;
                        }
                    }
                }
                else
                {
                    AppenCharacter(builder, console, nextKey.KeyChar);
                    previousParseResult = parser.ParseCommandLine(builder.ToString(), commandCache);
                    tabbedCount = 0;
                }
            }

            //Parse one last time to let the caller know exactly what's in the Console screen
            return parser.ParseCommandLine(builder.ToString(), commandCache);
        }

        private void AppenCharacter(StringBuilder builder, ICommandingConsole console, char keyChar)
        {
            builder.Append(keyChar);
            console.AppendCharacter(keyChar);
        }

        private void AppendText(StringBuilder builder, ICommandingConsole console, string textToAppend)
        {
            builder.Append(textToAppend);
            console.Write(textToAppend);
        }

        private void TrimEndCharacters(StringBuilder builder, ICommandingConsole console, int charactersToRemove)
        {
            var indexToStartRemoving = builder.Length - charactersToRemove;

            if (indexToStartRemoving >= 0 && charactersToRemove > 0)
            {
                builder.Remove(indexToStartRemoving, charactersToRemove);
                console.TrimEndCharacters(charactersToRemove);
            }
        }

        private string GetTextToAddToConsoleAtTabCount(ParseCommandLineResult previousParseResult, int tabbedCount)
        {
            if (previousParseResult.PossibleTextsToAutofill.Any())
            {
                var isTabbingForParameter = previousParseResult.Command != null;

                var indextOfNextText = tabbedCount % previousParseResult.PossibleTextsToAutofill.Count;

                var result = previousParseResult.PossibleTextsToAutofill[indextOfNextText];

                if (isTabbingForParameter)
                {
                    result = previousParseResult.Command.ParameterHeader + result;
                }

                return result;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}

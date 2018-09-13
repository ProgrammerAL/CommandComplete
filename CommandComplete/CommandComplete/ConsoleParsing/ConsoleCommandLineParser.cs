using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace CommandComplete.ConsoleParsing
{
    public class ConsoleCommandLineParser
    {
        private const int MaxHistory = 10;
        private readonly LinkedList<ParseCommandLineResult> _pastResults;

        public ConsoleCommandLineParser() => _pastResults = new LinkedList<ParseCommandLineResult>();

        public ParseCommandLineResult ParseCommandLine(ICommandCache commandCache, ICommandingConsole console)
        {
            var builder = new StringBuilder();
            int tabbedCount = 0;
            int currentHistoryBufferIndex = 0;

            var parser = new CommandLineParser();
            ParseCommandLineResult previousParseResult = parser.ParseCommandLine(string.Empty, commandCache);

            bool isParsingFromCommandLine = true;
            while (isParsingFromCommandLine)
            {
                ConsoleKeyInfo nextKey = console.GetGetKey(true);

                switch (nextKey.Key)
                {
                    case ConsoleKey.Enter:
                        console.Write(Environment.NewLine);
                        isParsingFromCommandLine = false;
                        break;
                    case ConsoleKey.Backspace:
                        TrimEndCharacters(builder, console, 1);
                        currentHistoryBufferIndex = 0;
                        break;
                    case ConsoleKey.Escape:
                        TrimCurrentCommandPiece(builder, console);
                        currentHistoryBufferIndex = 0;
                        break;
                    case ConsoleKey.Tab:
                        tabbedCount = AttemptToParseCommandInConsole(previousParseResult, tabbedCount, builder, console);
                        break;
                    case ConsoleKey.UpArrow:
                        ReplaceConsoleCommandWithHistoryItem(builder, currentHistoryBufferIndex, console);
                        currentHistoryBufferIndex = IncreaseHistoryCount(currentHistoryBufferIndex);
                        break;
                    case ConsoleKey.DownArrow:
                        currentHistoryBufferIndex = DecreaseHistoryCount(currentHistoryBufferIndex);
                        ReplaceConsoleCommandWithHistoryItem(builder, currentHistoryBufferIndex, console);
                        break;
                    default:
                        AppendCharacter(builder, console, nextKey.KeyChar);
                        previousParseResult = parser.ParseCommandLine(builder.ToString(), commandCache);
                        tabbedCount = 0;
                        currentHistoryBufferIndex = 0;
                        break;
                }
            }

            //Parse one last time to let the caller know exactly what's in the Console screen
            ParseCommandLineResult finalParsedCommandLine = parser.ParseCommandLine(builder.ToString(), commandCache);
            AddParsedLineToHistory(finalParsedCommandLine);

            return finalParsedCommandLine;
        }

        private void ReplaceConsoleCommandWithHistoryItem(StringBuilder builder, int historyIndex, ICommandingConsole console)
        {
            ParseCommandLineResult commandToRepalceWith = _pastResults.Skip(historyIndex).Take(1).Single();
            RepalceText(builder, console, commandToRepalceWith.ToString());
        }

        private int DecreaseHistoryCount(int currentHistoryBufferIndex)
        {
            currentHistoryBufferIndex--;
            if (currentHistoryBufferIndex < 0)
            {
                var historyBufferCount = CalculateHistoryBufferCount();
                currentHistoryBufferIndex = historyBufferCount - 1;
            }

            return currentHistoryBufferIndex;
        }

        private int IncreaseHistoryCount(int currentHistoryBufferIndex)
        {
            currentHistoryBufferIndex++;
            var historyBufferCount = CalculateHistoryBufferCount();
            if (currentHistoryBufferIndex == historyBufferCount)
            {
                currentHistoryBufferIndex = 0;
            }

            return currentHistoryBufferIndex;
        }

        private int CalculateHistoryBufferCount() => Math.Min(_pastResults.Count, MaxHistory);

        /// <summary>
        /// Tries to parse out a command from the string entered so far in the commpand line
        /// </summary>
        /// <param name="previousParseResult">The result of parsing the last time we tried parsing. When the previous key was pressed.</param>
        /// <param name="tabbedCount">How many times in a row the tab key has bit pressed</param>
        /// <param name="builder">StringBuilder instance that will be parsed from. Note: This will probably be modified</param>
        /// <param name="console">Instance of the console screen to output any text to. For example: When tab is hit and we add some text. That text is added to the string builder and console screen.</param>
        /// <returns>Tabbed count after attempting to parse command line</returns>
        private int AttemptToParseCommandInConsole(ParseCommandLineResult previousParseResult, int tabbedCount, StringBuilder builder, ICommandingConsole console)
        {
            int charactersToRemove = previousParseResult.RemainingText.Length;

            if (tabbedCount > 0)
            {
                //User hit tab more 2 or more times in a row, so remove the text they previously entered before adding something else
                string previousTextTabbedIntoConsole = GetTextToAddToConsoleAtTabCount(previousParseResult, tabbedCount - 1);
                charactersToRemove = previousTextTabbedIntoConsole.Length;
            }

            string textToAppend = GetTextToAddToConsoleAtTabCount(previousParseResult, tabbedCount);
            if (!string.IsNullOrEmpty(textToAppend))
            {
                TrimEndCharacters(builder, console, charactersToRemove);
                AppendText(builder, console, textToAppend);
                tabbedCount++;
            }

            return tabbedCount;
        }

        private void TrimCurrentCommandPiece(StringBuilder builder, ICommandingConsole console)
        {
            //Delete the current part of the command string
            //  Command Name, parameter name, parameter value, etc

            string currentText = builder.ToString();
            int lastIndexOfSpace = currentText.LastIndexOf(' ');
            if (lastIndexOfSpace == -1)
            {
                TrimEndCharacters(builder, console, currentText.Length);
            }
            else
            {
                int charactersToRemove = currentText.Length - lastIndexOfSpace;
                TrimEndCharacters(builder, console, charactersToRemove);
            }
        }

        private void AppendCharacter(StringBuilder builder, ICommandingConsole console, char keyChar)
        {
            builder.Append(keyChar);
            console.AppendCharacter(keyChar);
        }

        private void AppendText(StringBuilder builder, ICommandingConsole console, string textToAppend)
        {
            builder.Append(textToAppend);
            console.Write(textToAppend);
        }

        private void RepalceText(StringBuilder builder, ICommandingConsole console, string textToAppend)
        {
            console.TrimEndCharacters(builder.Length);
            builder.Clear();

            builder.Append(textToAppend);
            console.Write(textToAppend);
        }

        private void TrimEndCharacters(StringBuilder builder, ICommandingConsole console, int charactersToRemove)
        {
            int indexToStartRemoving = builder.Length - charactersToRemove;

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
                bool isTabbingForParameter = previousParseResult.Command != null;

                int indextOfNextText = tabbedCount % previousParseResult.PossibleTextsToAutofill.Count;

                string result = previousParseResult.PossibleTextsToAutofill[indextOfNextText];

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

        private void AddParsedLineToHistory(ParseCommandLineResult result)
        {
            _pastResults.AddFirst(result);

            //Make sure we only have MaxHistory items in list
            int overageCount = _pastResults.Count - MaxHistory;
            for (int i = 0; i < overageCount; i++)
            {
                _pastResults.RemoveLast();
            }
        }
    }
}

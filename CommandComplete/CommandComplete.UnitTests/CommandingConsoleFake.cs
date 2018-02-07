using CommandComplete.ConsoleParsing;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace CommandComplete.UnitTests
{
    public class CommandingConsoleFake : ICommandingConsole
    {
        private readonly StringBuilder _builder;
        private IImmutableList<ConsoleKeyInfo> _keyInfos;
        private int _nextKeyInfoIndex;

        public CommandingConsoleFake()
        {
            _builder = new StringBuilder();
            _nextKeyInfoIndex = 0;
        }

        public void AppendCharacter(char keyChar)
        {
            _builder.Append(keyChar);
        }

        public ConsoleKeyInfo GetGetKey(bool hideKeyPressedInConsole)
        {
            var nextKeyInfo = _keyInfos[_nextKeyInfoIndex];
            _nextKeyInfoIndex++;
            return nextKeyInfo;
        }

        public void TrimEndCharacters(int length)
        {
            var indexToStartRemoving = _builder.Length - length;
            _builder.Remove(indexToStartRemoving, length);
        }

        public void Write(string textToAddToConsole)
        {
            _builder.Append(textToAddToConsole);
        }

        public void SetConsoleKeyInfoOrder(IEnumerable<ConsoleKeyInfo> keyInfos)
        {
            _keyInfos = keyInfos.ToImmutableList();
        }
    }
}

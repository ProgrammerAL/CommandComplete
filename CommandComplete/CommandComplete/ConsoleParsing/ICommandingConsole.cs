using System;
using System.Collections.Generic;
using System.Text;

namespace CommandComplete.ConsoleParsing
{
    public interface ICommandingConsole
    {
        /// <param name="hideKeyPressedInConsole">True to not display the key pressed in the console window</param>
        ConsoleKeyInfo GetGetKey(bool hideKeyPressedInConsole);

        void TrimEndCharacters(int lengthToDelete);
        void Write(string textToAddToConsole);
        void AppendCharacter(char keyChar);
    }
}

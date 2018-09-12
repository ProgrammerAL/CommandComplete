using System;
using System.Runtime.Serialization;

namespace CommandComplete
{
    [Serializable]
    public class DuplicateCommandNameException : Exception
    {
        public DuplicateCommandNameException(string commandName) : this() => CommandName = commandName;

        public DuplicateCommandNameException(string commandName, string message) : base(message) => CommandName = commandName;

        public DuplicateCommandNameException(string commandName, string message, Exception innerException) : base(message, innerException) => CommandName = commandName;

        protected DuplicateCommandNameException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public DuplicateCommandNameException() : base()
        {
        }

        public DuplicateCommandNameException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public string CommandName { get; }
    }
}
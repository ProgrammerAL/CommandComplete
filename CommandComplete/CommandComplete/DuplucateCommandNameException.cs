using System;
using System.Runtime.Serialization;

namespace CommandComplete
{
    [Serializable]
    internal class DuplucateCommandNameException : Exception
    {
        public DuplucateCommandNameException(string commandName)
        {
            CommandName = commandName;
        }

        public DuplucateCommandNameException(string commandName, string message) : base(message)
        {
            CommandName = commandName;
        }

        public DuplucateCommandNameException(string commandName, string message, Exception innerException) : base(message, innerException)
        {
            CommandName = commandName;
        }

        protected DuplucateCommandNameException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public string CommandName
        {
            get;
            private set;
        }
    }
}
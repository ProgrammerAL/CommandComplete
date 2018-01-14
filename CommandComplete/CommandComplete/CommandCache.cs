using System;
using System.Collections.Immutable;
using System.Linq;

namespace CommandComplete
{
    public class CommandCache : ICommandCache
    {
        public CommandCache()
        {
            PossibleCommands = ImmutableList.Create<Command>();
        }

        public IImmutableList<Command> PossibleCommands { get; private set; }

        public void AddCommand(Command command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (PossibleCommands.Any(x => x.Name == command.Name))
            {
                throw new Exception($"Can't add command with duplicate key of {command.Name}");
            }

            PossibleCommands = PossibleCommands.Add(command);
        }
    }
}

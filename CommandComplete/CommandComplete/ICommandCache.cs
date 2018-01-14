using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Linq;

namespace CommandComplete
{
    public interface ICommandCache
    {
        IImmutableList<Command> PossibleCommands { get; }

        void AddCommand(Command command);
    }
}

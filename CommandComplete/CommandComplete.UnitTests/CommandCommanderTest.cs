using Shouldly;
using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace CommandComplete.UnitTests
{
    public class ConsoleCommanderTest
    {
        [Fact]
        public void WhenAddingNullCommand_AssertExceptionThrown()
        {
            var commander = new CommandCache();

            Should.Throw<ArgumentNullException>(() => commander.AddCommand(null));
        }

        [Fact]
        public void WhenAddingNewCommandWithSameName_AssertException()
        {
            var commander = new CommandCache();

            var command1 = new Command("SomeCommand", '-', null, null);
            var command2 = new Command("SomeCommand", '-', null, null);

            commander.AddCommand(command1);

            Should.Throw<Exception>(() => commander.AddCommand(command2));
        }
    }
}

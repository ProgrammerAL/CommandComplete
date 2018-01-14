using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;

namespace CommandComplete.UnitTests
{
    public class CommandCommandTest
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("Some Name")]
        [InlineData("-SomeName")]
        [InlineData("\\SomeName")]
        [InlineData("/SomeName")]
        [InlineData("\tSomeName")]
        [InlineData("\bSomeName")]
        public void WhenCreatingCommandWithInvalidName_AssertException(string name)
        {
            Assert.Throws<ArgumentException>(() => new Command(name, '-', null, null));
        }

        [Fact]
        public void WhenCreatingCommand_AssertProperties()
        {
            var command = new Command("SomeCommand", '-', "Help Text", new[] { new ParameterOption("Param1" , true, "Parameter 1") });

            Assert.Equal("SomeCommand", command.Name);
            Assert.Equal("Help Text", command.HelpText);
            Assert.Equal("Param1", command.Parameters.Single().Name);
        }
    }
}

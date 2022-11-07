using NUnit.Framework;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Tests.Contracts;
using Rebus.Transport.InMem;

namespace Rebus.Events.Tests;

[TestFixture]
public class TestReadmeCode : FixtureBase
{
    [Test]
    public void JustTheSnippet()
    {
        var activator = new BuiltinHandlerActivator();

        Using(activator);

        Configure.With(activator)
            .Transport(t => t.UseInMemoryTransport(new InMemNetwork(), "readme"))
            .Events(e =>
            {
                e.BeforeMessageSent += (bus, headers, message, context) =>
                {
                    headers["x-custom-header"] = "wohoo";
                };
            });
    }
}
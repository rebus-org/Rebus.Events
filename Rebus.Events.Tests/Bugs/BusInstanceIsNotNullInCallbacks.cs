using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Tests.Contracts;
using Rebus.Tests.Contracts.Extensions;
using Rebus.Transport.InMem;
#pragma warning disable CS1998

namespace Rebus.Events.Tests.Bugs;

[TestFixture]
public class BusInstanceIsNotNullInCallbacks : FixtureBase
{
    [Test]
    public async Task ItWorks()
    {
        var activator = new BuiltinHandlerActivator();

        Using(activator);

        var messageHandled = new ManualResetEvent(false);
        var callbackCalled = new ManualResetEvent(false);
        var busInstanceWas4Real = default(bool?);

        activator.Handle<string>(async str => messageHandled.Set());

        Configure.With(activator)
            .Transport(t => t.UseInMemoryTransport(new InMemNetwork(), "not-null-baby"))
            .Events(e =>
            {
                e.AfterMessageHandled += (bus, headers, message, context, args) =>
                {
                    busInstanceWas4Real = bus != null;
                    callbackCalled.Set();
                };
            })
            .Start();

        await activator.Bus.SendLocal("HEJ MED DIG");

        messageHandled.WaitOrDie(TimeSpan.FromSeconds(2));
        callbackCalled.WaitOrDie(TimeSpan.FromSeconds(2));

        Assert.That(busInstanceWas4Real.HasValue, Is.True, 
            "The bool was not even set, so the callback was not executed");
            
        Assert.That(busInstanceWas4Real, Is.True, 
            "Apparently the bus instance in the callback was not set");
    }
}
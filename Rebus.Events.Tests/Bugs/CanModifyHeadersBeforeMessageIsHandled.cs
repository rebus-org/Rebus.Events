using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Pipeline;
using Rebus.Tests.Contracts;
using Rebus.Tests.Contracts.Extensions;
using Rebus.Transport.InMem;
// ReSharper disable AccessToDisposedClosure
#pragma warning disable CS1998

namespace Rebus.Events.Tests.Bugs;

[TestFixture]
public class CanModifyHeadersBeforeMessageIsHandled : FixtureBase
{
    [Test]
    public async Task SeeIfItWorks()
    {
        static void RebusBeforeMessageHandled(IBus bus, Dictionary<string, string> headers, object message, IncomingStepContext context, MessageHandledEventHandlerArgs args)
        {
            headers["my-test"] = "test";
        }

        using var activator = new BuiltinHandlerActivator();
        using var gotTheHeaderAsExpected = new ManualResetEvent(initialState: false);

        activator.Handle<string>(async (_, context, _) =>
        {
            if (context.Message.Headers.TryGetValue("my-test", out var value) && value == "test")
            {
                gotTheHeaderAsExpected.Set();
            }
        });

        var bus = Configure.With(activator)
            .Transport(t => t.UseInMemoryTransport(new InMemNetwork(), "doesn't matter"))
            .Events(e => e.BeforeMessageHandled += RebusBeforeMessageHandled)
            .Start();

        await bus.SendLocal("HEJ MED DIG");

        gotTheHeaderAsExpected.WaitOrDie(timeout: TimeSpan.FromSeconds(5));
    }
}
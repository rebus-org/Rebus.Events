using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Rebus.Config;
using Rebus.Tests.Contracts.Extensions;
using Rebus.Tests.Contracts.Utilities;

// ReSharper disable ArgumentsStyleLiteral

#pragma warning disable 1998

namespace Rebus.Events.Tests.Events;

[TestFixture]
public class TestMessageHandled : BusFixtureBase
{
    [Test]
    public async Task IsCalledAroundHandlingAnIncomingMessage()
    {
        var events = new ConcurrentQueue<string>();

        ConfigureBus(configurer => configurer.Events(e =>
        {
            e.BeforeMessageHandled += (bus, headers, message, context, args) =>
            {
                events.Enqueue("BEFORE");
            };

            e.AfterMessageHandled += (bus, headers, message, context, args) =>
            {
                events.Enqueue("AFTER");
            };
        }));

        Handle<string>(async str =>
        {
            events.Enqueue("MSG!");
        });

        await Bus.SendLocal("hej med dig min ven!!!");

        await events.WaitUntil(c => c.Count == 3, timeoutSeconds: 2);

        Assert.That(events, Is.EqualTo(new[]
        {
            "BEFORE",
            "MSG!",
            "AFTER"
        }));
    }

    [Test]
    public async Task CanBeUsedToSwallowAnException()
    {
        ConfigureBus(c => c.Events(e =>
        {
            e.AfterMessageHandled += (bus, headers, message, context, args) =>
            {
                var exception = context.Load<Exception>();
                if (exception == null) return;

                if (exception is AbandonedMutexException)
                {
                    args.IgnoreException = true;
                }
            };
        }));

        Handle<string>(async str => throw new AbandonedMutexException("NO ACCESS!!"));

        Handle<DateTime>(async dateTime => throw new AbandonedMutexException("WE DO NOT CARE BUT IT IS CLEARLY GONE!!!!"));

        await Bus.SendLocal("hej");
        await Bus.SendLocal(DateTime.Now);

        await Task.Delay(2000);

        var lines = LoggerFactory.ToList();

        Assert.That(lines.Any(l => l.Text.Contains("AbandonedMutexException")), Is.False);
    }
}
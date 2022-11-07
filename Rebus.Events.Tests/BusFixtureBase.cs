using System;
using System.Threading.Tasks;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Pipeline;
using Rebus.Tests.Contracts;
using Rebus.Tests.Contracts.Utilities;
using Rebus.Transport.InMem;
// ReSharper disable ArgumentsStyleLiteral

namespace Rebus.Events.Tests;

public abstract class BusFixtureBase : FixtureBase
{
    protected ListLoggerFactory LoggerFactory;
    protected InMemNetwork Network;

    BuiltinHandlerActivator Activator;
    Lazy<IBus> LazyBus;

    protected void ConfigureBus(Action<RebusConfigurer> configure)
    {
        Activator = new BuiltinHandlerActivator();
        Network = new InMemNetwork();
        LoggerFactory = new ListLoggerFactory(outputToConsole: true);

        Using(Activator);

        var rebusConfigurer = Configure.With(Activator)
            .Logging(l => l.Use(LoggerFactory))
            .Transport(t => t.UseInMemoryTransport(Network, "event-test"));

        configure(rebusConfigurer);

        var starter = rebusConfigurer
            .Options(o => o.LogPipeline(true))
            .Create();

        LazyBus = new(() => starter.Start());
    }

    protected IBus Bus => LazyBus.Value;

    protected void Handle<TMessage>(Func<TMessage, Task> handler) => Activator.Handle<TMessage>(handler);

    protected void Handle<TMessage>(Func<IBus, IMessageContext, TMessage, Task> handler) => Activator.Handle<TMessage>(handler);
}
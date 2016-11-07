using System;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Tests.Contracts;
using Rebus.Tests.Contracts.Utilities;
using Rebus.Transport.InMem;
// ReSharper disable ArgumentsStyleLiteral

namespace Rebus.Events.Tests
{
    public abstract class BusFixtureBase : FixtureBase
    {
        protected InMemNetwork Network;
        protected BuiltinHandlerActivator Activator;
        protected ListLoggerFactory LoggerFactory;

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

            rebusConfigurer
                .Options(o => o.LogPipeline(true))
                .Start();
        }
    }
}
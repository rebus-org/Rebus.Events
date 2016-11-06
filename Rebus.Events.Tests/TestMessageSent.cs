using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Pipeline;
using Rebus.Pipeline.Send;
using Rebus.Tests.Contracts;
using Rebus.Tests.Contracts.Utilities;
using Rebus.Transport.InMem;
#pragma warning disable 1998

namespace Rebus.Events.Tests
{
    [TestFixture]
    public class TestMessageSent : FixtureBase
    {
        InMemNetwork _network;
        BuiltinHandlerActivator _activator;

        protected override void SetUp()
        {
            _activator = new BuiltinHandlerActivator();
            _network = new InMemNetwork();

            Using(_activator);
        }

        [Test]
        public void IsCalledAroundMessageSend()
        {
            var counter = new SharedCounter(1);

            ConfigureBus(c =>
            {
                c.Events(e =>
                {
                    e.BeforeMessageSent += (bus, headers, message, context) =>
                    {
                        headers["before!"] = "hej";
                    };

                    e.AfterMessageSent += (bus, headers, message, context) =>
                    {
                        headers["after!"] = "hej igen";
                    };
                });
            });

            _activator.Handle<string>(async (bus, context, message) =>
            {
                if (!context.Headers.ContainsKey("before!"))
                {
                    throw new AssertionException("Did not find 'before!' key in headers");
                }

                if (context.Headers.ContainsKey("after!"))
                {
                    throw new AssertionException("Found unexpected 'after!' key in headers");
                }

                counter.Decrement();
            });

            _activator.Bus.SendLocal("hej med dig min ven!!!!");

            counter.WaitForResetEvent(2);
        }

        void ConfigureBus(Action<RebusConfigurer> configure)
        {
            var rebusConfigurer = Configure.With(_activator)
                .Transport(t => t.UseInMemoryTransport(_network, "event-test"));

            configure(rebusConfigurer);

            rebusConfigurer
                .Options(o => o.LogPipeline(true))
                .Start();
        }
    }
}

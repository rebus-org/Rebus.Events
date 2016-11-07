using NUnit.Framework;
using Rebus.Config;
using Rebus.Tests.Contracts.Utilities;

#pragma warning disable 1998

namespace Rebus.Events.Tests.Events
{
    [TestFixture]
    public class TestMessageSent : BusFixtureBase
    {
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

            Activator.Handle<string>(async (bus, context, message) =>
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

            Activator.Bus.SendLocal("hej med dig min ven!!!!");

            counter.WaitForResetEvent(2);
        }
    }
}

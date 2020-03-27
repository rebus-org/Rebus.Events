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

namespace Rebus.Events.Tests.Events
{
    [TestFixture]
    public class TestSingleDispatch : BusFixtureBase
    {
        [Test]
        public async Task WhenEventsIsCalledMultipleTimesOnlyOneTimeEachEventIsCalled()
        {
            var events = new ConcurrentQueue<string>();
            int beforeCount = 0;
            int afterCount = 0;
            ConfigureBus(configurer =>
                configurer
                    .Events(e =>
                    {
                        e.BeforeMessageHandled += (bus, headers, message, context, args) =>
                        {
                            headers.Add("BEFORE", "test");
                            ++beforeCount;
                        };
                    })
                    .Events(e =>
                    {
                        e.BeforeMessageHandled += (bus, headers, message, context, args) =>
                        {
                            headers.Add("ANOTHER_BEFORE_HEADER", "test");
                            ++beforeCount;
                        };
                    })
                    .Events(e =>
                    {
                        e.AfterMessageHandled += (bus, headers, message, context, args) =>
                        {
                            headers.Add("AFTER", "test");
                            ++afterCount;
                        };
                    })
                    .Events(e =>
                    {
                        e.AfterMessageHandled += (bus, headers, message, context, args) =>
                        {
                            headers.Add("ANOTHER_AFTER_HEADER", "test");
                            ++afterCount;
                        };
                    }));

            int handleCount = 0;
            Activator.Handle<string>(async str => { ++handleCount; });

            await Activator.Bus.SendLocal("single-dispatch");

            await events.WaitUntil(c => handleCount == 1, timeoutSeconds: 2);

            Assert.AreEqual(2, beforeCount);
            Assert.AreEqual(2, afterCount);
            Assert.AreEqual(1, handleCount);
        }
    }
}
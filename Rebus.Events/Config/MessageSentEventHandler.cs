using System.Collections.Generic;
using Rebus.Bus;
using Rebus.Pipeline;

namespace Rebus.Config
{
    public delegate void MessageSentEventHandler(IBus bus, Dictionary<string, string> headers, object message, OutgoingStepContext context);
}
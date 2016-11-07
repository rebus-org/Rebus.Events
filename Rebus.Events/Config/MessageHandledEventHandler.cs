using System.Collections.Generic;
using Rebus.Bus;
using Rebus.Pipeline;

namespace Rebus.Config
{
    /// <summary>
    /// Signature for an event handler that can be registered before/after handling messages
    /// </summary>
    public delegate void MessageHandledEventHandler(IBus bus, Dictionary<string, string> headers, object message, IncomingStepContext context, MessageHandledEventHandlerArgs args);
}
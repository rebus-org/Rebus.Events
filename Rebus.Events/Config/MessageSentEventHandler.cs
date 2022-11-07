using System.Collections.Generic;
using Rebus.Bus;
using Rebus.Pipeline;

namespace Rebus.Config;

/// <summary>
/// Signature for an event handler that can be registered before/after a message is sent
/// </summary>
public delegate void MessageSentEventHandler(IBus bus, Dictionary<string, string> headers, object message, OutgoingStepContext context);
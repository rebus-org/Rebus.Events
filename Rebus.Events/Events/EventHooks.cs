using System.Collections.Generic;
using Rebus.Config;

namespace Rebus.Events;

class EventHooks
{
    public readonly List<MessageSentEventHandler> BeforeMessageSent = new();
    public readonly List<MessageSentEventHandler> AfterMessageSent = new();
    public readonly List<MessageHandledEventHandler> BeforeMessageHandled = new();
    public readonly List<MessageHandledEventHandler> AfterMessageHandled = new();
}
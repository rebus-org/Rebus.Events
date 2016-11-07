using System.Collections.Generic;
using Rebus.Config;

namespace Rebus.Events
{
    class EventHooks
    {
        public readonly List<MessageSentEventHandler> BeforeMessageSent = new List<MessageSentEventHandler>();
        public readonly List<MessageSentEventHandler> AfterMessageSent = new List<MessageSentEventHandler>();
        public readonly List<MessageHandledEventHandler> BeforeMessageHandled = new List<MessageHandledEventHandler>();
        public readonly List<MessageHandledEventHandler> AfterMessageHandled = new List<MessageHandledEventHandler>();
    }
}
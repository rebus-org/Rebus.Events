using System;
using System.Collections.Generic;
using Rebus.Events;

namespace Rebus.Config
{
    public class EventConfigurer
    {
        readonly List<MessageSentEventHandler> _beforeMessageSent = new List<MessageSentEventHandler>();
        readonly List<MessageSentEventHandler> _afterMessageSent = new List<MessageSentEventHandler>();

        public event MessageSentEventHandler BeforeMessageSent
        {
            add { _beforeMessageSent.Add(value); }
            remove { throw new NotSupportedException("It is not possible to remove an event handler once it has been added"); }
        }

        public event MessageSentEventHandler AfterMessageSent
        {
            add { _afterMessageSent.Add(value); }
            remove { throw new NotSupportedException("It is not possible to remove an event handler once it has been added"); }
        }

        internal void CopyTo(EventHooks eventHooks)
        {
            eventHooks.BeforeMessageSent.AddRange(_beforeMessageSent);

            eventHooks.AfterMessageSent.AddRange(_afterMessageSent);
        }
    }
}

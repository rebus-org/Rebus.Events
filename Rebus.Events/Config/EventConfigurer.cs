using System;
using System.Collections.Generic;
using Rebus.Events;
using Rebus.Pipeline;

namespace Rebus.Config
{
    /// <summary>
    /// Configuration object that exposes ordinary .NET events to enable callback registration using familiar APIs
    /// </summary>
    public class EventConfigurer
    {
        readonly List<MessageSentEventHandler> _beforeMessageSent = new List<MessageSentEventHandler>();
        readonly List<MessageSentEventHandler> _afterMessageSent = new List<MessageSentEventHandler>();
        readonly List<MessageHandledEventHandler> _beforeMessageHandled = new List<MessageHandledEventHandler>();
        readonly List<MessageHandledEventHandler> _afterMessageHandled = new List<MessageHandledEventHandler>();

        /// <summary>
        /// Event raised as the first thing before a message is dispatched to the message handler(s)
        /// </summary>
        public event MessageHandledEventHandler BeforeMessageHandled
        {
            add { _beforeMessageHandled.Add(value); }
            remove { throw new NotSupportedException("It is not possible to remove an event handler once it has been added"); }
        }

        /// <summary>
        /// Event raised after a message has been dispatched to the message handler(s). In case an exception was caught, the exception is available in the <see cref="IncomingStepContext"/> passed to the event handler. The exception can be swallowed by setting <see cref="MessageHandledEventHandlerArgs.IgnoreException"/> to true
        /// </summary>
        public event MessageHandledEventHandler AfterMessageHandled
        {
            add { _afterMessageHandled.Add(value); }
            remove { throw new NotSupportedException("It is not possible to remove an event handler once it has been added"); }
        }

        /// <summary>
        /// Event raised as the first thing before a message is sent. Can be used to e.g. add custom headers.
        /// </summary>
        public event MessageSentEventHandler BeforeMessageSent
        {
            add { _beforeMessageSent.Add(value); }
            remove { throw new NotSupportedException("It is not possible to remove an event handler once it has been added"); }
        }

        /// <summary>
        /// Event raised after a message has been sent. The event is raised when the message has been successfully sent or added
        /// to the list of outgoing messages for the current transaction context. This means that the message may/may not actually
        /// have been sent.
        /// </summary>
        public event MessageSentEventHandler AfterMessageSent
        {
            add { _afterMessageSent.Add(value); }
            remove { throw new NotSupportedException("It is not possible to remove an event handler once it has been added"); }
        }

        internal void CopyTo(EventHooks eventHooks)
        {
            eventHooks.BeforeMessageSent.AddRange(_beforeMessageSent);
            eventHooks.AfterMessageSent.AddRange(_afterMessageSent);
            eventHooks.BeforeMessageHandled.AddRange(_beforeMessageHandled);
            eventHooks.AfterMessageHandled.AddRange(_afterMessageHandled);
        }
    }
}

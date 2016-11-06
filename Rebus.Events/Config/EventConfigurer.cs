using System;
using System.Collections.Generic;
using Rebus.Events;

namespace Rebus.Config
{
    /// <summary>
    /// Configuration object that exposes ordinary .NET events to enable callback registration using familiar APIs
    /// </summary>
    public class EventConfigurer
    {
        readonly List<MessageSentEventHandler> _beforeMessageSent = new List<MessageSentEventHandler>();
        readonly List<MessageSentEventHandler> _afterMessageSent = new List<MessageSentEventHandler>();

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
        }
    }
}

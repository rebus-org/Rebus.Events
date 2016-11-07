using System;

namespace Rebus.Config
{
    /// <summary>
    /// Event args for message handled events
    /// </summary>
    public class MessageHandledEventHandlerArgs : EventArgs
    {
        /// <summary>
        /// Gets/sets whether to ignore an exception caught while dispatching the message
        /// </summary>
        public bool IgnoreException { get; set; }
    }
}
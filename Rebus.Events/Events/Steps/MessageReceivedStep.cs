using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Messages;
using Rebus.Pipeline;

namespace Rebus.Events.Steps
{
    class MessageReceivedStep : IIncomingStep, IInitializable
    {
        readonly List<MessageHandledEventHandler> _beforeMessageHandled;
        readonly List<MessageHandledEventHandler> _afterMessageHandled;
        readonly Func<IBus> _busFactory;
        IBus _bus;

        public MessageReceivedStep(List<MessageHandledEventHandler> beforeMessageHandled, List<MessageHandledEventHandler> afterMessageHandled, Func<IBus> busFactory)
        {
            _beforeMessageHandled = beforeMessageHandled.ToList();
            _afterMessageHandled = afterMessageHandled.ToList();
            _busFactory = busFactory;
        }

        public void Initialize()
        {
            _bus = _busFactory();
        }

        public async Task Process(IncomingStepContext context, Func<Task> next)
        {
            var message = context.Load<Message>();
            var args = new MessageHandledEventHandlerArgs();

            foreach (var e in _beforeMessageHandled)
            {
                e(_bus, message.Headers, message.Body, context, args);
            }

            try
            {
                await next();

                foreach (var e in _afterMessageHandled)
                {
                    e(_bus, message.Headers, message.Body, context, args);
                }
            }
            catch (Exception exception)
            {
                context.Save(exception);

                foreach (var e in _afterMessageHandled)
                {
                    e(_bus, message.Headers, message.Body, context, args);
                }

                if (!args.IgnoreException)
                {
                    throw;
                }
            }
        }
    }
}
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
    [StepDocumentation("Invokes Before- and AfterMessageSent handlers")]
    class MessageSentStep : IOutgoingStep, IInitializable
    {
        readonly Func<IBus> _busFactory;
        readonly List<MessageSentEventHandler> _beforeMessageSent;
        readonly List<MessageSentEventHandler> _afterMessageSent;
        IBus _bus;

        public MessageSentStep(List<MessageSentEventHandler> beforeMessageSent, List<MessageSentEventHandler> afterMessageSent, Func<IBus> busFactory)
        {
            _busFactory = busFactory;
            _beforeMessageSent = beforeMessageSent.ToList();
            _afterMessageSent = afterMessageSent.ToList();
        }

        public void Initialize()
        {
            _bus = _busFactory();
        }

        public async Task Process(OutgoingStepContext context, Func<Task> next)
        {
            var message = context.Load<Message>();

            foreach (var e in _beforeMessageSent)
            {
                e(_bus, message.Headers, message.Body, context);
            }

            try
            {
                await next();

                foreach (var e in _afterMessageSent)
                {
                    e(_bus, message.Headers, message.Body, context);
                }
            }
            catch (Exception exception)
            {
                context.Save(exception);

                foreach (var e in _afterMessageSent)
                {
                    e(_bus, message.Headers, message.Body, context);
                }

                throw;
            }
        }
    }
}
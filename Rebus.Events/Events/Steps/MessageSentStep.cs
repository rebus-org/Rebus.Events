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
    class MessageSentStep : IOutgoingStep
    {
        readonly List<MessageSentEventHandler> _beforeMessageSent;
        readonly List<MessageSentEventHandler> _afterMessageSent;
        readonly Lazy<IBus> _bus;

        public MessageSentStep(List<MessageSentEventHandler> beforeMessageSent, List<MessageSentEventHandler> afterMessageSent, Func<IBus> busFactory)
        {
            _bus = new Lazy<IBus>(busFactory);
            _beforeMessageSent = beforeMessageSent.ToList();
            _afterMessageSent = afterMessageSent.ToList();
        }

        public async Task Process(OutgoingStepContext context, Func<Task> next)
        {
            var message = context.Load<Message>();

            foreach (var e in _beforeMessageSent)
            {
                e(_bus.Value, message.Headers, message.Body, context);
            }

            try
            {
                await next();

                foreach (var e in _afterMessageSent)
                {
                    e(_bus.Value, message.Headers, message.Body, context);
                }
            }
            catch (Exception exception)
            {
                context.Save(exception);

                foreach (var e in _afterMessageSent)
                {
                    e(_bus.Value, message.Headers, message.Body, context);
                }

                throw;
            }
        }
    }
}
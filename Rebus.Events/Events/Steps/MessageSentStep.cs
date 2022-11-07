using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Messages;
using Rebus.Pipeline;

namespace Rebus.Events.Steps;

[StepDocumentation("Invokes Before- and AfterMessageSent handlers")]
class MessageSentStep : IOutgoingStep
{
    readonly Lazy<IBus> _lazyBus;
    readonly List<MessageSentEventHandler> _beforeMessageSent;
    readonly List<MessageSentEventHandler> _afterMessageSent;

    public MessageSentStep(List<MessageSentEventHandler> beforeMessageSent, List<MessageSentEventHandler> afterMessageSent, Func<IBus> lazyBus)
    {
        _lazyBus = new(lazyBus);
        _beforeMessageSent = beforeMessageSent.ToList();
        _afterMessageSent = afterMessageSent.ToList();
    }

    public async Task Process(OutgoingStepContext context, Func<Task> next)
    {
        var message = context.Load<Message>();
        var bus = _lazyBus.Value;
        
        foreach (var e in _beforeMessageSent)
        {
            e(bus, message.Headers, message.Body, context);
        }

        try
        {
            await next();

            foreach (var e in _afterMessageSent)
            {
                e(bus, message.Headers, message.Body, context);
            }
        }
        catch (Exception exception)
        {
            context.Save(exception);

            foreach (var e in _afterMessageSent)
            {
                e(bus, message.Headers, message.Body, context);
            }

            throw;
        }
    }
}
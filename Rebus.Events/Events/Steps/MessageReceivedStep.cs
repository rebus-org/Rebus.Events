using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Messages;
using Rebus.Pipeline;

namespace Rebus.Events.Steps;

class MessageReceivedStep : IIncomingStep
{
    readonly List<MessageHandledEventHandler> _beforeMessageHandled;
    readonly List<MessageHandledEventHandler> _afterMessageHandled;
    readonly Lazy<IBus> _lazyBus;

    public MessageReceivedStep(List<MessageHandledEventHandler> beforeMessageHandled, List<MessageHandledEventHandler> afterMessageHandled, Func<IBus> lazyBus)
    {
        _beforeMessageHandled = beforeMessageHandled.ToList();
        _afterMessageHandled = afterMessageHandled.ToList();
        _lazyBus = new(lazyBus);
    }

    public async Task Process(IncomingStepContext context, Func<Task> next)
    {
        var message = context.Load<Message>();
        var args = new MessageHandledEventHandlerArgs();
        var bus = _lazyBus.Value;
        var headers = message.Headers;
        var body = message.Body;

        foreach (var e in _beforeMessageHandled)
        {
            e(bus, headers, body, context, args);
        }

        try
        {
            await next();

            foreach (var e in _afterMessageHandled)
            {
                e(bus, headers, body, context, args);
            }
        }
        catch (Exception exception)
        {
            context.Save(exception);

            foreach (var e in _afterMessageHandled)
            {
                e(bus, headers, body, context, args);
            }

            if (!args.IgnoreException)
            {
                throw;
            }
        }
    }
}
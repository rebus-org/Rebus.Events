using System;
using Rebus.Bus;
using Rebus.Events;
using Rebus.Events.Steps;
using Rebus.Pipeline;
using Rebus.Pipeline.Receive;

namespace Rebus.Config;

/// <summary>
/// Convenience configuration extensions for adding callbacks to various Rebus events
/// </summary>
public static class EventConfigurationExtensions
{
    /// <summary>
    /// Configures event callbacks
    /// </summary>
    public static RebusConfigurer Events(this RebusConfigurer configurer, Action<EventConfigurer> configureEvents)
    {
        if (configurer == null) throw new ArgumentNullException(nameof(configurer));
        if (configureEvents == null) throw new ArgumentNullException(nameof(configureEvents));

        configurer.Options(o =>
        {
            if (!o.Has<EventHooks>())
            {
                o.Register(_ => new EventHooks());

                o.Decorate<IPipeline>(c =>
                {
                    var pipeline = c.Get<IPipeline>();
                    var eventHooks = c.Get<EventHooks>();

                    var step = new MessageSentStep(eventHooks.BeforeMessageSent, eventHooks.AfterMessageSent, c.Get<IBus>);

                    return new PipelineStepConcatenator(pipeline)
                        .OnSend(step, PipelineAbsolutePosition.Front);
                });

                o.Decorate<IPipeline>(c =>
                {
                    var pipeline = c.Get<IPipeline>();
                    var eventHooks = c.Get<EventHooks>();

                    var receiveStep = new MessageReceivedStep(eventHooks.BeforeMessageHandled, eventHooks.AfterMessageHandled, c.Get<IBus>);

                    return new PipelineStepInjector(pipeline)
                        .OnReceive(receiveStep, PipelineRelativePosition.Before, typeof(DispatchIncomingMessageStep));
                });
            }

            o.Decorate(c =>
            {
                var eventConfigurerInstance = new EventConfigurer();
                configureEvents(eventConfigurerInstance);

                var eventHooks = c.Get<EventHooks>();
                eventConfigurerInstance.CopyTo(eventHooks);

                return eventHooks;
            });
        });

        return configurer;
    }
}
using System;
using Rebus.Bus;
using Rebus.Events;
using Rebus.Events.Steps;
using Rebus.Pipeline;

namespace Rebus.Config
{
    public static class EventConfigurationExtensions
    {
        public static RebusConfigurer Events(this RebusConfigurer configurer, Action<EventConfigurer> eventConfigurer)
        {
            configurer.Options(o =>
            {
                if (!o.Has<EventHooks>())
                {
                    Console.WriteLine("REGISTERING EVENT HOOKS");
                    o.Register(c => new EventHooks());
                }

                o.Decorate(c =>
                {
                    Console.WriteLine("APPLYING DECORATOR TO CALL BACK TO THE EVENT CONFIGURER");
                    var eventHooks = c.Get<EventHooks>();
                    var eventConfigurerInstance = new EventConfigurer();
                    eventConfigurer(eventConfigurerInstance);

                    eventConfigurerInstance.CopyTo(eventHooks);

                    return eventHooks;
                });

                o.Decorate(c =>
                {
                    Console.WriteLine("INSTANTIATING EVENT HOOKS");
                    c.Get<EventHooks>();
                    return c.Get<IBus>();
                });




                o.Decorate<IPipeline>(c =>
                {
                    var pipeline = c.Get<IPipeline>();
                    var eventHooks = c.Get<EventHooks>();
                    var step = new MessageSentStep(eventHooks.BeforeMessageSent, eventHooks.AfterMessageSent, c.Get<IBus>);
                    return new PipelineStepConcatenator(pipeline)
                        .OnSend(step, PipelineAbsolutePosition.Front);
                });
            });

            return configurer;
        }
    }
}
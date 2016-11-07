# Rebus.Events

[![install from nuget](https://img.shields.io/nuget/v/Rebus.Events.svg?style=flat-square)](https://www.nuget.org/packages/Rebus.Events)

Provides configuration extensions that allow for easily hooking into Rebus in various places.

Here is how you would add custom headers to all outgoing messages:

    Configure.With(...)
        .(...)
        .Events(e =>
        {
            e.BeforeMessageSent += (bus, headers, message, context) =>
            {
                headers["x-custom-header"] = "wohoo";
            };
        });

The following events are available:

* `BeforeMessageSent`: Raised before each message is sent, allowing for mutating the message and even replacing it with something else if you want
* `AfterMessageSent`: Raised after each message has been sent (or added to the transaction's list of outgoing messages)
* `BeforeMessageHandled`: Raised before an incoming message is dispatched to handlers
* `AfterMessageHandled`: Raised after an incoming message is dispatched to handlers
* _more to come_
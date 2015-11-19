# MassTransit.MessageData.MongoDb

MongoDb message data repository implementation for MassTransit

[![install from nuget](http://img.shields.io/nuget/v/MassTransit.MessageData.MongoDb.svg?style=flat-square)](https://www.nuget.org/packages/MassTransit.MessageData.MongoDb)
[![downloads](http://img.shields.io/nuget/dt/MassTransit.MessageData.MongoDb.svg?style=flat-square)](https://www.nuget.org/packages/MassTransit.MessageData.MongoDb)
[![Build status](https://ci.appveyor.com/api/projects/status/xxxryqtofg5k5d1s/branch/master?svg=true)](https://ci.appveyor.com/project/Liberis/masstransit-mongodb/branch/master)

##Getting Started
MassTransit.MessageData.MongoDb can be installed via the package manager console by executing the following commandlet:

```powershell
PM> Install-Package MassTransit.MessageData.MongoDb
```

Once we have the package installed, we can then use our favourite IoC container to wire up the MongoDb repository using one of the following constructor overloads:

###1. MongoUrl
```csharp
var repo = new MongoMessageDataRepository(new MongoUrl("mongodb://localhost/masstransitTest"));
```

###2. Connection String & Database
```csharp
var repo = new MongoMessageDataRepository("mongodb://localhost", "masstransitTest");
```

This is the object that MassTransit requires to implement `IMessageDataRepository`.

##Usage

###Storing message data in Mongo

First, we need to create a model for the message data.

```csharp
using MassTransit;

public class BigTestMessage
{
    public MessageData<byte[]> Blob { get; set; }
}
```

Next, we need to configure the bus with with an endpoint.

```csharp
var messages = new ConcurrentBag<BigTestMessage>();

var busControl = MassTransit.Bus.Factory.CreateUsingInMemory(cfg =>
{
    cfg.ReceiveEndpoint("test-" + new Guid().ToString(), ep =>
    {
        ep.Handler<BigTestMessage>(context =>
        {
            messages.Add(context.Message);
            return Task.FromResult(0);
        });

        ep.UseMessageData<BigTestMessage>(MessageDataRepository.Instance);
    });
});

var busHandle = busControl.Start();
await busHandle.Ready;
```

Then we can setup our `IMessageDataRepository` instance.

```csharp
public static class MessageDataRepository
{
    public static IMessageDataRepository Instance { get; } =
        new MongoMessageDataRepository(new MongoUrl("mongodb://localhost/masstransitTest"));
}
```

Finally, we can create and populate our message with data, then publish the message to the bus.

```csharp
var blob = new byte[] {111, 2, 234, 12, 99};

var message = new BigTestMessage { Blob = await MessageDataRepository.Instance.PutBytes(blob); }

busControl.Publish(message);
```

## Contributing

1. Fork
1. Hack!
1. Pull Request

# MassTransit.MongoDb
This repository contains integrations for MassTransit V3 with MongoDB

[![Build status](https://ci.appveyor.com/api/projects/status/vfhjcx6kpfd54ugo/branch/master?svg=true)](https://ci.appveyor.com/project/Liberis/masstransit-mongodb/branch/master)

The integrations so far:

## MassTransit.MessageData.MongoDb

This package allows big data messages to be stored in MongoDB, this stops your messaging system getting clogged up with big payloads.

[![install from nuget](http://img.shields.io/nuget/v/MassTransit.MessageData.MongoDb.svg?style=flat-square)](https://www.nuget.org/packages/MassTransit.MessageData.MongoDb)
[![downloads](http://img.shields.io/nuget/dt/MassTransit.MessageData.MongoDb.svg?style=flat-square)](https://www.nuget.org/packages/MassTransit.MessageData.MongoDb)


### Getting Started
MassTransit.MessageData.MongoDb can be installed via the package manager console by executing the following commandlet:

```powershell

PM> Install-Package MassTransit.MessageData.MongoDb

```

Once we have the package installed, we can create a `MongoMessageDataRepository` using one of the following constructors:

```csharp

var repository = new MongoMessageDataRepository(new MongoUrl("mongodb://localhost/masstransitTest"));

```

Or

```csharp

var repository = new MongoMessageDataRepository("mongodb://localhost", "masstransitTest");

```

#### Sending a Big Message

Say we have a `BigMessage` that has a  `BigPayload` property of a type of `MessageData<byte[]>`:

```csharp

public class BigMessage
{
    public string SomeProperty1 { get; set; }

    public int SomeProperty2 { get; set; }

    public MessageData<byte[]> BigPayload { get; set; }
}

```

When we create the message we need to call our `MongoMessageDataRepository` to put the big payload into MongoDB, which in turn passes back a `MessageData<byte[]>`:

```csharp
var blob = new byte[] {111, 2, 234, 12, 99};

var bigPayload = await repository.PutBytes(blob);

var message = new BigMessage
{
    SomeProperty1 = "Other property that will get passed on message queue",
    SomeProperty2 = 12,
    BigPayload =  bigPayload
};

```

We can then publish/send it like any other MassTransit message:

```csharp

busControl.Publish(message);

```

#### Receiving a Big Message

To receive a message with a big payload we need to configure our endpoint to use the repository for a given message type:

```csharp

var busControl = MassTransit.Bus.Factory.CreateUsingInMemory(cfg =>
{
    cfg.ReceiveEndpoint("my_awesome_endpoint", ep =>
    {
        // Normal Receive Endpoint Config...

        ep.UseMessageData<BigMessage>(repository);
    });
});

```

Then, with the magic wiring from MassTransit we can consume the message inside a consumer with the following:
```csharp

public class BigMessageConsumer : IConsumer<BigMessage>
{
    public async Task Consume(ConsumeContext<BigMessage> context)
    {
        var bigPayload = await context.Message.BigPayload.Value;

        // Do something with the big payload...
    }
}

```

## Cleaning up Expired GridFS Data

Expired GridFS data can be removed by running the following script:

```javascript
// deleteExpiredMessageData.js

var docs = db.getMongo().getDB("masstransit");
var now = new Date().toISOString();

var cursor = docs.fs.files.find({"metadata.expiration" : {$lte : new Date(now)}});

cursor.forEach(function (toDelete) {
	var id = toDelete._id;
	docs.fs.chunks.remove({files_id : id});
	docs.fs.files.remove({_id : id});
});    
```

Alternatively, you can import the `CreateDeleteExpiredMassTransitMessageDataTask.xml` file into Windows Task Scheduler and configure the script's arguments so that expired documents are deleted on a schedule.

## ToDo

* Saga Repository

## Contribute

1. Fork
1. Hack!
1. Pull Request

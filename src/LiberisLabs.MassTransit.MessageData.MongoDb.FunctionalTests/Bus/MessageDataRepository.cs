using MassTransit.MessageData;
using MongoDB.Driver;

namespace LiberisLabs.MassTransit.MessageData.MongoDb.FunctionalTests.Bus
{
    public static class MessageDataRepository
    {
        public static IMessageDataRepository Instance { get; } =
            new MongoMessageDataRepository(new MongoUrl("mongodb://localhost/masstransitTest"));
    }
}

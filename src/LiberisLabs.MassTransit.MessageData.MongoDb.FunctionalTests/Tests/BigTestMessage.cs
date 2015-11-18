using MassTransit;

namespace LiberisLabs.MassTransit.MessageData.MongoDb.FunctionalTests.Tests
{
    public class BigTestMessage
    {
        public MessageData<byte[]> Blob { get; set; }
    }
}
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using LiberisLabs.MassTransit.MessageData.MongoDb.FunctionalTests.Bus;
using NUnit.Framework;
using MassTransit;

namespace LiberisLabs.MassTransit.MessageData.MongoDb.FunctionalTests.Tests
{
    [TestFixture]
    public class SendingABigMessageTest
    {
        private readonly ConcurrentBag<BigTestMessage> _bigTestMessages = new ConcurrentBag<BigTestMessage>();
        private BusHandle _busHandle;
        private IBusControl _busControl;
        private byte[] _expectedBlob;

        [TestFixtureSetUp]
        public void GivenARunningBusThatIsListeningToABigTestMessageThatIsUsingMongoMessageDataRepository()
        {
            _busControl = global::MassTransit.Bus.Factory.CreateUsingInMemory(cfg =>
            {
                cfg.ReceiveEndpoint("test-" + new Guid().ToString(), ep =>
                {
                    ep.Handler<BigTestMessage>((context) => { _bigTestMessages.Add(context.Message);
                                                                return Task.FromResult(0);
                    });

                    ep.UseMessageData<BigTestMessage>(MessageDataRepository.Instance);
                });
            });

            _busHandle = _busControl.Start();
            _busHandle.Ready.Wait();
        }

        [SetUp]
        public void WhenPublishingABigTestMessage()
        {
            _expectedBlob = new byte[]
            {
                111, 2, 234, 23, 23, 234, 235
            };

            var bigTestMessage = new BigTestMessage()
            {
                Blob = MessageDataRepository.Instance.PutBytes(_expectedBlob).Result
            };

            _busControl.Publish(bigTestMessage);
        }

        [Test]
        public async Task ThenBigTestMessageContainsData()
        {
            BigTestMessage actual = null;

            Assert.That(() => (actual =_bigTestMessages.SingleOrDefault()), Is.Not.Null.After(1000, 100));

            Assert.That(await actual.Blob.Value, Is.EqualTo(_expectedBlob));
        }
    }
}

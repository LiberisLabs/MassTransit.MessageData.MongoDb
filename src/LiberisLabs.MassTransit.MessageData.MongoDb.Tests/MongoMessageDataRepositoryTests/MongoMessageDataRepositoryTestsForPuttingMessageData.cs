using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace LiberisLabs.MassTransit.MessageData.MongoDb.Tests.MongoMessageDataRepositoryTests
{
    public class MongoMessageDataRepositoryTestsForPuttingMessageData
    {
        private GridFSBucket _bucket;
        private byte[] _expected;
        private ObjectId _id;

        [TestFixtureSetUp]
        public void GivenAMongoMessageDataRepository_WhenPuttingMessageData()
        {
            var db = new MongoClient().GetDatabase("messagedatastoretests");
            _bucket = new GridFSBucket(db);
            var fixture = new Fixture();
            _expected = fixture.Create<byte[]>();
            var resolver = new Mock<IMongoMessageUriResolver>();
            resolver.Setup(m => m.Resolve(It.IsAny<ObjectId>())).Returns((ObjectId x) => new Uri("urn:" + x));
            var sut = new MongoMessageDataRepository(resolver.Object, _bucket);

            using (var stream = new MemoryStream(_expected))
            {
                var uri = sut.Put(stream).GetAwaiter().GetResult();
                _id = new ObjectId(uri.AbsoluteUri.Split(':').Last());
            }
        }
        
        [Test]
        public async Task ThenMessageStoredAsExpected()
        {
            var result = await _bucket.DownloadAsBytesAsync(_id);

            Assert.That(result, Is.EqualTo(_expected));
        }

        [TestFixtureTearDown]
        public void Kill()
        {
            _bucket.DropAsync().GetAwaiter().GetResult();
        }
    }
}
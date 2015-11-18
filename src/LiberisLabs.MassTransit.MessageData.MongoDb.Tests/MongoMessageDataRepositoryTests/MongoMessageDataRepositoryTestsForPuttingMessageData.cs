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
        private Mock<IFileNameCreator> _nameCreator;
        private Mock<IMongoMessageUriResolver> _resolver;

        [TestFixtureSetUp]
        public void GivenAMongoMessageDataRepository_WhenPuttingMessageData()
        {
            var db = new MongoClient().GetDatabase("messagedatastoretests");
            _bucket = new GridFSBucket(db);
            var fixture = new Fixture();
            _expected = fixture.Create<byte[]>();
            _resolver = new Mock<IMongoMessageUriResolver>();
            _resolver.Setup(m => m.Resolve(It.IsAny<ObjectId>())).Returns((ObjectId x) => new Uri("urn:" + x));
            _nameCreator = new Mock<IFileNameCreator>();
            _nameCreator.Setup(m => m.CreateFileName()).Returns(fixture.Create<string>());
            var sut = new MongoMessageDataRepository(_resolver.Object, _bucket, _nameCreator.Object);

            using (var stream = new MemoryStream(_expected))
            {
                var uri = sut.Put(stream).GetAwaiter().GetResult();
                _id = new ObjectId(uri.AbsoluteUri.Split(':').Last());
            }
        }

        [Test]
        public void ThenResolverCalled()
        {
            _resolver.Verify(m => m.Resolve(_id));
        }

        public void ThenNameCreatorCalled()
        {
            _nameCreator.Verify(m => m.CreateFileName());
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
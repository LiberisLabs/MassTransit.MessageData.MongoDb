using System;
using System.IO;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace LiberisLabs.MassTransit.MessageData.MongoDb.Tests.MongoMessageDataRepositoryTests
{
    [TestFixture]
    public class MongoMessageDataRepositoryTestsForPuttingMessageData
    {
        private GridFSBucket _bucket;
        private byte[] _expectedData;
        private Mock<IFileNameCreator> _nameCreator;
        private Mock<IMongoMessageUriResolver> _resolver;
        private Uri _expectedUri;
        private Uri _actualUri;
        private string _filename;

        [TestFixtureSetUp]
        public void GivenAMongoMessageDataRepository_WhenPuttingMessageData()
        {
            var db = new MongoClient().GetDatabase("messagedatastoretests");
            _bucket = new GridFSBucket(db);

            var fixture = new Fixture();
            _expectedData = fixture.Create<byte[]>();
            _expectedUri = fixture.Create<Uri>();

            _resolver = new Mock<IMongoMessageUriResolver>();
            _resolver.Setup(m => m.Resolve(It.IsAny<ObjectId>()))
                .Returns((ObjectId x) => _expectedUri);

            _nameCreator = new Mock<IFileNameCreator>();
            _filename = fixture.Create<string>();
            _nameCreator.Setup(m => m.CreateFileName()).Returns(_filename);

            var sut = new MongoMessageDataRepository(_resolver.Object, _bucket, _nameCreator.Object);

            using (var stream = new MemoryStream(_expectedData))
            {
                _actualUri = sut.Put(stream).GetAwaiter().GetResult();
            }
        }

        [Test]
        public void ThenTheCorrectUriIsReturned()
        {
            Assert.That(_actualUri, Is.EqualTo(_expectedUri));
        }
        
        [Test]
        public async Task ThenMessageStoredAsExpected()
        {
            var result = await _bucket.DownloadAsBytesByNameAsync(_filename);

            Assert.That(result, Is.EqualTo(_expectedData));
        }

        [TestFixtureTearDown]
        public void Kill()
        {
            _bucket.DropAsync().GetAwaiter().GetResult();
        }
    }
}
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiberisLabs.MassTransit.MessageData.MongoDb.Helpers;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace LiberisLabs.MassTransit.MessageData.MongoDb.Tests.MongoMessageDataRepositoryTests
{
    [TestFixture]
    public class MongoMessageDataRepositoryTestsForPuttingMessageDataWithExpiration
    {
        private GridFSBucket _bucket;
        private byte[] _expected;
        private ObjectId _resultId;
        private TimeSpan _expectedTtl;
        private DateTime _now;

        [TestFixtureSetUp]
        public void GivenAMongoMessageDataRepository_WhenPuttingMessageDataWithExpiration()
        {
            var db = new MongoClient().GetDatabase("messagedatastoretests");
            _bucket = new GridFSBucket(db);

            _now = DateTime.UtcNow;
            SystemDateTime.Set(_now);

            var fixture = new Fixture();
            _expected = fixture.Create<byte[]>();

            var resolver = new Mock<IMongoMessageUriResolver>();
            resolver.Setup(m => m.Resolve(It.IsAny<ObjectId>())).Returns((ObjectId x) => new Uri("urn:" + x));

            var nameCreator = new Mock<IFileNameCreator>();
            nameCreator.Setup(m => m.CreateFileName()).Returns(fixture.Create<string>());

            var sut = new MongoMessageDataRepository(resolver.Object, _bucket, nameCreator.Object);
            _expectedTtl = TimeSpan.FromHours(1);

            using (var stream = new MemoryStream(_expected))
            {
                var uri = sut.Put(stream, _expectedTtl).GetAwaiter().GetResult();
                _resultId = new ObjectId(uri.AbsoluteUri.Split(':').Last());
            }
        }
        
        [Test]
        public async Task ThenMessageStoredAsExpected()
        {
            var result = await _bucket.DownloadAsBytesAsync(_resultId);

            Assert.That(result, Is.EqualTo(_expected));
        }

        [Test]
        public async Task ThenExpirationSetAsExpected()
        {
            var cursor = await _bucket.FindAsync(Builders<GridFSFileInfo>.Filter.Eq("_id", _resultId));
            var list = await cursor.ToListAsync();
            var doc = list.Single();

            var expiration = doc.Metadata["expiration"].ToUniversalTime();
            Assert.That(expiration, Is.EqualTo(_now.Add(_expectedTtl)).Within(1).Milliseconds);
        }

        [TestFixtureTearDown]
        public void Kill()
        {
            SystemDateTime.Reset();

            _bucket.DropAsync().GetAwaiter().GetResult();
        }
    }
}
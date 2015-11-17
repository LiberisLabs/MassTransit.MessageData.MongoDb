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
    public class MongoMessageDataRepositoryTestsForGettingMessageData
    {
        private GridFSBucket _bucket;
        private Stream _result;
        private byte[] _expected;

        [TestFixtureSetUp]
        public void GivenAMongoMessageDataRepository_WhenGettingMessageData()
        {
            var db = new MongoClient().GetDatabase("messagedatastoretests");
            _bucket = new GridFSBucket(db);
            var fixture = new Fixture();
            _expected = fixture.Create<byte[]>();
            var objectId = SeedBucket(_expected).GetAwaiter().GetResult();
            var resolver = new Mock<IMongoMessageUriResolver>();
            resolver.Setup(m => m.Resolve(It.IsAny<Uri>())).Returns(objectId);
            var sut = new MongoMessageDataRepository(resolver.Object, _bucket);
            _result = sut.Get(It.IsAny<Uri>()).GetAwaiter().GetResult();
        }

        private async Task<ObjectId> SeedBucket(byte[] seed)
        {
            return await _bucket.UploadFromBytesAsync(Path.GetRandomFileName(), seed);
        }
        
        [Test]
        public async Task ThenStreamReturnedAsExpected()
        {
            var result = new byte[_result.Length];
            await _result.ReadAsync(result, 0, result.Length);
            Assert.That(result, Is.EqualTo(_expected));
        }

        [TestFixtureTearDown]
        public void Kill()
        {
            _bucket.DropAsync().GetAwaiter().GetResult();
        }
    }
}
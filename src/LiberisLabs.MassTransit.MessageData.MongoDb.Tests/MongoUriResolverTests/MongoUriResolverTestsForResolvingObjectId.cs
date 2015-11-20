using System;
using MongoDB.Bson;
using NUnit.Framework;

namespace LiberisLabs.MassTransit.MessageData.MongoDb.Tests.MongoUriResolverTests
{
    public class MongoUriResolverTestsForResolvingObjectId
    {
        private Uri _expected;
        private Uri _result;

        [SetUp]
        public void GivenAMongoMessageUriResolver_WhenResolvingAnObjectId()
        {
            var objectId = ObjectId.GenerateNewId();
            _expected = new Uri(string.Format($"urn:mongodb:gridfs:{objectId}"));
            var sut = new MongoMessageUriResolver();

            _result = sut.Resolve(objectId);
        }

        [Test]
        public void ThenUriFormattedCorrectly()
        {
            Assert.That(_result, Is.EqualTo(_expected));
        }
    }
}
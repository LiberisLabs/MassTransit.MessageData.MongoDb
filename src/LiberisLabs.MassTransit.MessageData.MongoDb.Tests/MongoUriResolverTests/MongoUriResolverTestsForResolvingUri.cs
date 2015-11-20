using System;
using MongoDB.Bson;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace LiberisLabs.MassTransit.MessageData.MongoDb.Tests.MongoUriResolverTests
{
    [TestFixture]
    public class MongoUriResolverTestsForResolvingUri
    {
        private ObjectId _expected;
        private ObjectId _result;

        [SetUp]
        public void GivenAMongoMessageUriResolver_WhenResolvingObjectIdFromUri()
        {
            _expected = ObjectId.GenerateNewId();
            var uri = new Uri(string.Format($"urn:mongodb:gridfs:{_expected}"));
            var sut = new MongoMessageUriResolver();

            _result = sut.Resolve(uri);
        }

        [Test]
        public void ThenExpectedObjectIdReturnedFromUri()
        {
            Assert.That(_result, Is.EqualTo(_expected));
        }
    }
}

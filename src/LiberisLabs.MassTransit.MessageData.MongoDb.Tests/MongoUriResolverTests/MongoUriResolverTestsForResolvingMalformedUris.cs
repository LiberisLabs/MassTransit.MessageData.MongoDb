using System;
using MongoDB.Bson;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace LiberisLabs.MassTransit.MessageData.MongoDb.Tests.MongoUriResolverTests
{
    [TestFixture("abc:mongodb:gridfs:12345")]
    [TestFixture("urn:mongodb:gridfs:somethingelse:12345")]
    [TestFixture("urn:mongodb:gridfsthing:12345")]

    public class MongoUriResolverTestsForResolvingMalformedUris
    {
        private Exception _exception;
        private readonly Uri _uri;

        public MongoUriResolverTestsForResolvingMalformedUris(string uri)
        {
            _uri = new Uri(uri);
        }

        [SetUp]
        public void GivenAMongoMessageUriResolver_WhenResolvingMalformedUris()
        {
            var sut = new MongoMessageUriResolver();

            try
            {
                sut.Resolve(_uri);
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        [Test]
        public void ThenExceptionIsUriFormatException()
        {
            Assert.That(_exception, Is.TypeOf<UriFormatException>());
        }
    }
}

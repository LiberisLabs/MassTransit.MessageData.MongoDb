using System;
using MongoDB.Bson;

namespace LiberisLabs.MassTransit.MessageData.MongoDb
{
    public class MongoMessageUriResolver : IMongoMessageUriResolver
    {
        private const string _scheme = "urn";
        private const string _system = "mongodb";
        private const string _specification = "gridfs";
        private readonly string _format = string.Join(":", _system, _specification);

        public ObjectId Resolve(Uri uri)
        {
            if (uri.Scheme != _scheme)
                throw new UriFormatException("Incorrect scheme");

            var tokens = uri.AbsolutePath.Split(':');

            if (tokens.Length != 3 || !uri.AbsoluteUri.StartsWith($"{_format}:"))
                throw new UriFormatException($"Urn is not in the correct format. Use '{_format}:{{resourceId}}'");

            return ObjectId.Parse(tokens[2]);
        }

        public Uri Resolve(ObjectId id)
        {
            return new Uri($"{_scheme}:{_format}:{id}");
        }
    }
}
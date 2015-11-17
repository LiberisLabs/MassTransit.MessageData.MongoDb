using System;
using MongoDB.Bson;

namespace LiberisLabs.MassTransit.MessageData.MongoDb
{
    public class MongoMessageUriConvertor : IMongoMessageUriConvertor
    {
        public ObjectId Build(Uri uri)
        {
            if (uri.Scheme != "urn")
                throw new UriFormatException("Incorrect scheme");

            var tokens = uri.AbsolutePath.Split(':');

            if (tokens.Length != 3 || !uri.AbsolutePath.StartsWith("mongodb:gridfs:"))
                throw new UriFormatException("Urn is not in the correct format. Use 'urn:mongodb:gridfs:{objectId}'");

            return ObjectId.Parse(tokens[2]);
        }
    }
}
using System;
using MongoDB.Bson;

namespace LiberisLabs.MassTransit.MessageData.MongoDb
{
    public class MongoMessageUriBuilder : IMongoMessageUriBuilder
    {
        public Uri Build(ObjectId id)
        {
            return new Uri(String.Format(new MongoMessageUriFormatter(), "{0}", id));
        }
    }
}
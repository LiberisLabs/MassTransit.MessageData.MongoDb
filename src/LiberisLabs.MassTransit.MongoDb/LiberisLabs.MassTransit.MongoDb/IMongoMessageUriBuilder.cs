using System;
using MongoDB.Bson;

namespace LiberisLabs.MassTransit.MessageData.MongoDb
{
    public interface IMongoMessageUriBuilder
    {
        Uri Build(ObjectId id);
    }
}
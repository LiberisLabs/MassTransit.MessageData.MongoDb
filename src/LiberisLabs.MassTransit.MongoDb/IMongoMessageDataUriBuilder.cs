using System;
using MongoDB.Bson;

namespace LiberisLabs.MassTransit.MessageData.MongoDb
{
    public interface IMongoMessageUriResolver
    {
        ObjectId Resolve(Uri uri);

        Uri Resolve(ObjectId id);
    }
}
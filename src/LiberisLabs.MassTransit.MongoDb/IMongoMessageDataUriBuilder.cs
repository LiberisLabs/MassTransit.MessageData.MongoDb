using System;
using MongoDB.Bson;

namespace LiberisLabs.MassTransit.MessageData.MongoDb
{
    public interface IMongoMessageUriConvertor
    {
        ObjectId Build(Uri uri);

        Uri Build(ObjectId id);
    }
}
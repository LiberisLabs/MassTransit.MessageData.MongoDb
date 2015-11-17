﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MassTransit.MessageData;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;

namespace LiberisLabs.MassTransit.MessageData.MongoDb
{
    public class MongoMessageDataRepository : IMessageDataRepository
    {
        private readonly IMongoMessageUriResolver _mongoMessageUriResolver;
        private readonly IGridFSBucket _gridFsBucket;

        public MongoMessageDataRepository(IMongoMessageUriResolver mongoMessageUriResolver, IGridFSBucket gridFsBucket)
        {
            _mongoMessageUriResolver = mongoMessageUriResolver;
            _gridFsBucket = gridFsBucket;
        }

        public async Task<Stream> Get(Uri address, CancellationToken cancellationToken = new CancellationToken())
        {
            var id = _mongoMessageUriResolver.Resolve(address);

            return await _gridFsBucket.OpenDownloadStreamAsync(id, cancellationToken: cancellationToken);
        }

        public async Task<Uri> Put(Stream stream, TimeSpan? timeToLive = null, CancellationToken cancellationToken = new CancellationToken())
        {
            var options = BuildGridFSUploadOptions(timeToLive);

            var id = await _gridFsBucket.UploadFromStreamAsync(Path.GetRandomFileName(), stream, options, cancellationToken);

            return _mongoMessageUriResolver.Resolve(id);
        }

        private GridFSUploadOptions BuildGridFSUploadOptions(TimeSpan? timeToLive)
        {
            if (timeToLive == null)
                return null;

            var metadata = new BsonDocument
            {
                {"expiration", DateTime.UtcNow.Add(timeToLive.Value)}
            };

            return new GridFSUploadOptions
            {
                Metadata = metadata
            };
        }
    }
}
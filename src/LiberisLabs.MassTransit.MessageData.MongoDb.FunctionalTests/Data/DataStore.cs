using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace LiberisLabs.MassTransit.MessageData.MongoDb.FunctionalTests.Data
{
    public static class DataStore
    {
        public static async Task<IList<GridFSFileInfo>> GetFileMetaData(string filename)
        {
            var gridFsBucket = GetGridFsBucket();

            return await (await gridFsBucket.FindAsync(Builders<GridFSFileInfo>.Filter.Eq(x => x.Filename, filename)))
                .ToListAsync();
        }
        public static async Task<byte[]> GetFile(string filename)
        {
            var gridFsBucket = GetGridFsBucket();

            return await gridFsBucket.DownloadAsBytesByNameAsync(filename);
        }

        private static GridFSBucket GetGridFsBucket()
        {
            var db = new MongoClient().GetDatabase("document");

            var gridFsBucket = new GridFSBucket(db);
            return gridFsBucket;
        }
    }
}

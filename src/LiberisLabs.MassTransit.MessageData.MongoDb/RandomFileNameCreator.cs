using System.IO;

namespace LiberisLabs.MassTransit.MessageData.MongoDb
{
    public class RandomFileNameCreator : IFileNameCreator
    {
        public string CreateFileName()
        {
            return Path.GetRandomFileName();
        }
    }
}
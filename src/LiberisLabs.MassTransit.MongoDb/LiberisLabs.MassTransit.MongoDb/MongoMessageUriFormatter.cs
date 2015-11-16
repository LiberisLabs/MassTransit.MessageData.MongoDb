using System;

namespace LiberisLabs.MassTransit.MessageData.MongoDb
{
    public class MongoMessageUriFormatter : IFormatProvider, ICustomFormatter
    {
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            return String.Format($"urn:mongodb:gridfs:{arg}", arg);
        }

        public object GetFormat(Type formatType)
        {
            if (formatType == typeof (ICustomFormatter))
                return this;

            return null;
        }
    }
}
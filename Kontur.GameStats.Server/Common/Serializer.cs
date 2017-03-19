using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Kontur.GameStats.Server
{
    public static class Serializer
    {
        private static readonly Dictionary<Type, DataContractJsonSerializer> _serializers =
            new Dictionary<Type, DataContractJsonSerializer>();

        public static void SerializeObject(object obj, Stream dataStream)
        {
            var objectType = obj.GetType();
            dataStream.Position = 0;

            if (!_serializers.ContainsKey(objectType))
                _serializers[objectType] = new DataContractJsonSerializer(objectType);

            _serializers[objectType].WriteObject(dataStream, obj);
        }

        public static object DeserializeObject(Type objectType, Stream dataStream)
        {
            dataStream.Position = 0;

            if (!_serializers.ContainsKey(objectType))
                _serializers[objectType] = new DataContractJsonSerializer(objectType);

            return _serializers[objectType].ReadObject(dataStream);
        }
    }
}
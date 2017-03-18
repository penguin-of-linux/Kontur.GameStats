using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Kontur.GameStats.Server
{
    public static class Serializer
    {
        private static Dictionary<Type, DataContractJsonSerializer> serializers =
            new Dictionary<Type, DataContractJsonSerializer>()
            {
                //{ typeof(ServerInfo), new DataContractJsonSerializer(typeof(ServerInfo)) },
                //{ typeof(MatchInfo), new DataContractJsonSerializer(typeof(MatchInfo)) },
                //{ typeof(Server), new DataContractJsonSerializer(typeof(Server)) }
            };

        public static void SerializeObject(object obj, Stream dataStream)
        {
            var objectType = obj.GetType();
            dataStream.Position = 0;

            if (!serializers.ContainsKey(objectType))
                serializers[objectType] = new DataContractJsonSerializer(objectType);

            serializers[objectType].WriteObject(dataStream, obj);
        }

        public static object DeserializeObject(Type objectType, Stream dataStream)
        {
            dataStream.Position = 0;

            if (!serializers.ContainsKey(objectType))
                serializers[objectType] = new DataContractJsonSerializer(objectType);

            return serializers[objectType].ReadObject(dataStream);
        }
    }
}
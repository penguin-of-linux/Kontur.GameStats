using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Kontur.GameStats.Server
{
    public class EndpointInfoCommand : ICommand, IGet, IPut
    {
        private readonly DataBase dataBase;

        public EndpointInfoCommand(DataBase dataBase)
        {
            this.dataBase = dataBase;
        }
        public bool IsParametersDefineCommand(string commandParameters)
        {
            var parameters = commandParameters.Split('/');
            var result = parameters[1] == "servers" && Common.IsEndpoint(parameters[2]) && parameters[3] == "info";

            return result;
        }

        public Stream Get(string commandParameter)
        {
            var serverName = commandParameter.Split('/')[2];
            var server = dataBase.GetServer(serverName);
            var dataStream = new MemoryStream();
            Serializer.SerializeObject(server.info, dataStream);

            return dataStream;
        }

        public void Put(string commandParameter, Stream dataStream)
        {
            var endpoint = commandParameter.Split('/')[2];
            var serverInfo = (ServerInfo) Serializer.DeserializeObject(typeof(ServerInfo), dataStream);
            dataBase.AddServer(new Server(endpoint, serverInfo));
        }
    }
}

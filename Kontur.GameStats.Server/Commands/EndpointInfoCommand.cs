using System.IO;

namespace Kontur.GameStats.Server
{
    public class EndpointInfoCommand : IGetCommand, IPutCommand
    {
        private readonly SimpleDataBase dataBase;

        public EndpointInfoCommand(SimpleDataBase dataBase)
        {
            this.dataBase = dataBase;
        }
        public bool IsParametersDefineCommand(string commandParameters)
        {
            var parameters = commandParameters.Split('/');

            return parameters[1] == "servers" && Common.IsEndpoint(parameters[2]) && parameters[3] == "info";
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

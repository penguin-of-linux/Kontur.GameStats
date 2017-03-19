using System.IO;

namespace Kontur.GameStats.Server.Commands
{
    public class ServerStatsCommand : IGetCommand
    {
        private readonly SimpleDataBase dataBase;

        public ServerStatsCommand(SimpleDataBase dataBase)
        {
            this.dataBase = dataBase;
        }
        public bool IsParametersDefineCommand(string commandParameters)
        {
            var parameters = commandParameters.Split('/');
            return parameters[1] == "servers" && Common.IsEndpoint(parameters[2]) && parameters[3] == "stats";
        }

        public Stream Get(string commandParameter)
        {
            var endpoint = commandParameter.Split('/')[2];
            var dataStream = new MemoryStream();

            Serializer.SerializeObject(dataBase.GetServerStats(endpoint), dataStream);

            return dataStream;
        }
    }
}
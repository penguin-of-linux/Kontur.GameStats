using System.IO;
using System.Linq;

namespace Kontur.GameStats.Server.Commands
{
    public class ServersInfoCommand : IGetCommand
    {
        private readonly SimpleDataBase dataBase;

        public ServersInfoCommand(SimpleDataBase dataBase)
        {
            this.dataBase = dataBase;
        }
        public bool IsParametersDefineCommand(string commandParameters)
        {
            var parameters = commandParameters.Split('/');

            return parameters[1] == "servers" && parameters[2] == "info";
        }

        public Stream Get(string commandParameter)
        {
            var dataStream = new MemoryStream();
            var servers = dataBase.GetAllServers().ToList();
            Serializer.SerializeObject(servers, dataStream);

            return dataStream;
        }
    }
}

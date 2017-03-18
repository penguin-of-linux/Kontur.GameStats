using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Kontur.GameStats.Server.Commands
{
    public class ServersInfoCommand : ICommand, IGet
    {
        private readonly DataBase dataBase;

        public ServersInfoCommand(DataBase dataBase)
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kontur.GameStats.Server.Commands
{
    public class PlayerStatsCommand : ICommand, IGet
    {
        private readonly DataBase dataBase;

        public PlayerStatsCommand(DataBase dataBase)
        {
            this.dataBase = dataBase;
        }

        public bool IsParametersDefineCommand(string commandParameters)
        {
            var parameters = commandParameters.Split('/');

            return parameters[1] == "players" && parameters[3] == "stats";
        }

        public Stream Get(string commandParameter)
        {
            var name = commandParameter.Split('/')[2];
            var dataStream = new MemoryStream();

            Serializer.SerializeObject(dataBase.GetPlayerStats(name), dataStream);

            return dataStream;
        }
    }
}

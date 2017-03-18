using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kontur.GameStats.Server.Commands
{
    public class BestPlayersCommand : ICommand, IGet
    {
        private readonly DataBase dataBase;

        public BestPlayersCommand(DataBase dataBase)
        {
            this.dataBase = dataBase;
        }

        public bool IsParametersDefineCommand(string commandParameters)
        {
            var parameters = commandParameters.Split('/');

            return parameters[1] == "reports" && parameters[2] == "best-players";
        }

        public Stream Get(string commandParameter)
        {
            var dataStream = new MemoryStream();
            var parameters = commandParameter.Split('/');
            var count = parameters.Length == 3 ? 5 : int.Parse(parameters[3]);

            Serializer.SerializeObject(dataBase.GetBestPlayers(count), dataStream);

            return dataStream;
        }
    }
}

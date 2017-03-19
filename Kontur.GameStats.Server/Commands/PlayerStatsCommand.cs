using System.IO;

namespace Kontur.GameStats.Server.Commands
{
    public class PlayerStatsCommand : IGetCommand
    {
        private readonly SimpleDataBase dataBase;

        public PlayerStatsCommand(SimpleDataBase dataBase)
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

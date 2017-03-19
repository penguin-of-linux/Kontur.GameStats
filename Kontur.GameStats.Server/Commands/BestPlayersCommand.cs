using System.IO;

namespace Kontur.GameStats.Server.Commands
{
    public class BestPlayersCommand : IGetCommand
    {
        private readonly SimpleDataBase dataBase;

        public BestPlayersCommand(SimpleDataBase dataBase)
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

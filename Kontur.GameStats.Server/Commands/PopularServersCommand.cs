using System.IO;

namespace Kontur.GameStats.Server.Commands
{
    public class PopularServersCommand : IGetCommand
    {
        private readonly SimpleDataBase dataBase;

        public PopularServersCommand(SimpleDataBase dataBase)
        {
            this.dataBase = dataBase;
        }

        public bool IsParametersDefineCommand(string commandParameters)
        {
            var parameters = commandParameters.Split('/');

            return parameters[1] == "reports" && parameters[2] == "popular-servers";
        }

        public Stream Get(string commandParameter)
        {
            var dataStream = new MemoryStream();
            var parameters = commandParameter.Split('/');
            var count = parameters.Length == 3 ? 5 : int.Parse(parameters[3]);

            Serializer.SerializeObject(dataBase.GetPopularServers(count), dataStream);

            return dataStream;
        }
    }
}

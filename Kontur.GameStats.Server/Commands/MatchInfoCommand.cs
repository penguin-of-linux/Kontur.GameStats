using System.IO;

namespace Kontur.GameStats.Server
{
    public class MatchInfoCommand : IPutCommand, IGetCommand
    {
        private readonly SimpleDataBase dataBase;

        public MatchInfoCommand(SimpleDataBase dataBase)
        {
            this.dataBase = dataBase;
        }
        public bool IsParametersDefineCommand(string commandParameters)
        {
            var folders = commandParameters.Split('/');

            return folders[1] == "servers" && Common.IsEndpoint(folders[2]) 
                && folders[3] == "matches" && Common.IsTimestamp(folders[4]); 
        }

        public void Put(string commandParameter, Stream dataStream)
        {
            var newMatch = (MatchInfo)Serializer.DeserializeObject(typeof(MatchInfo), dataStream);

            var endpoint = commandParameter.Split('/')[2];
            var time = commandParameter.Split('/')[4];

            dataBase.PutMatch(endpoint, time, newMatch);
        }

        public Stream Get(string commandParameter)
        {
            var endpoint = commandParameter.Split('/')[2];
            var match = dataBase.GetMatch(endpoint, commandParameter.Split('/')[4]);

            var dataStream = new MemoryStream();
            Serializer.SerializeObject(match, dataStream);

            return dataStream;
        }
    }
}
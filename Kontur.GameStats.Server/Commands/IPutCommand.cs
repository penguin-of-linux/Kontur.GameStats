using System.IO;

namespace Kontur.GameStats.Server
{
    public interface IPutCommand : ICommand
    {
        void Put(string commandParameter, Stream dataStream);
    }
}

using System.IO;

namespace Kontur.GameStats.Server
{
    public interface IGetCommand : ICommand
    {
        Stream Get(string commandParameter);
    }
}

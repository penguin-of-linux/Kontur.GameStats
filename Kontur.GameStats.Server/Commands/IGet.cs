using System.IO;

namespace Kontur.GameStats.Server
{
    public interface IGet
    {
        Stream Get(string commandParameter);
    }
}

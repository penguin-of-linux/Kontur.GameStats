using System.IO;

namespace Kontur.GameStats.Server
{
    public interface IPut
    {
        void Put(string commandParameter, Stream dataStream);
    }
}

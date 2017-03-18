namespace Kontur.GameStats.Server
{
    public interface ICommand
    {
        bool IsParametersDefineCommand(string commandParameters);
    }
}

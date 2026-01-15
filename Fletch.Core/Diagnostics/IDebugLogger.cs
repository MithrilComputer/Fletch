namespace Fletch.Core.Diagnostics
{
    public interface IDebugLogger
    {
        void Info(string message);

        void Warning(string message);

        void Error(string message, Exception? ex = null);
    }
}

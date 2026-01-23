namespace Fletch.Core.Diagnostics
{
    public interface IFletchContextLogger<T>
    {
        void Log(string message);

        void LogWarning(string message);

        void LogError(string message);

        void LogError(string message, Exception exception);

        void LogError(Exception exception);
    }
}

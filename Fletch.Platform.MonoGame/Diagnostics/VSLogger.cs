using Fletch.Core.Diagnostics;
using System.Diagnostics;

namespace Fletch.Platform.MonoGame.Diagnostics
{
    internal class VSLogger : IFletchLogger
    {
        public void Log(string message)
        {
            Debug.WriteLine($"[ Message ] {message}");
        }

        public void LogWarning(string message)
        {
            Debug.WriteLine($"[ !Warning! ] {message}");
        }

        public void LogError(string message)
        {
            Debug.WriteLine($"[ !ERROR! ] {message}");
        }

        public void LogError(Exception exception)
        {
            Debug.WriteLine($"[ !ERROR! ] Exception: {exception.GetType().Name}, {exception.Message} || Stack: {exception.StackTrace}");
        }

        public void LogError(string message, Exception exception)
        {
            Debug.WriteLine($"[ !ERROR! ] {message} || Exception: {exception.GetType().Name}, {exception.Message} || Stack: {exception.StackTrace}");
        }
    }
}

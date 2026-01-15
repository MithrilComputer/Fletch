using Fletch.Core.Diagnostics;
using System.Diagnostics;

namespace Fletch.Platform.MonoGame.Diagnostics
{
    internal class VSLogger : IDebugLogger
    {
        public void Error(string message, Exception? ex = null)
        {
            Debug.WriteLine($"[ Error!! ] {message}", ex);
        }

        public void Warning(string message)
        {
            Debug.WriteLine($"[ Warning! ] {message}");
        }

        public void Info(string message)
        {
            Debug.WriteLine($"[ Message ] {message}");
        }
    }
}

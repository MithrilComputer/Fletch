namespace Fletch.Core.Diagnostics
{
    public static class Log
    {
        internal static IDebugLogger Current { get; set; } = new NullDebugLogger();

        public static void Info(string msg) => Current.Info(msg);
        public static void Warning(string msg) => Current.Warning(msg);
        public static void Error(string msg, Exception? ex = null) => Current.Error(msg, ex);

        //Just using this to prevent null problems before the logger is assigned
        private sealed class NullDebugLogger : IDebugLogger
        {
            public void Info(string message) { }
            public void Warning(string message) { }
            public void Error(string message, Exception? ex = null) { }
        }
    }
}

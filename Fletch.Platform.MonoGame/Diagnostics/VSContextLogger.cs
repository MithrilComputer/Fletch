using Fletch.Core.Diagnostics;

namespace Fletch.Platform.MonoGame.Diagnostics
{
    internal class VSContextLogger<T> : IFletchContextLogger<T>
    {
        private readonly IFletchLogger inner;
        private readonly string context;

        public VSContextLogger(IFletchLogger inner)
        {
            this.inner = inner;
            context = typeof(T).Name;
        }

        public void Log(string message) =>
            inner.Log($"[{context}] {message}");

        public void LogWarning(string message) =>
            inner.LogWarning($"[{context}] {message}");

        public void LogError(string message) =>
            inner.LogError($"[{context}] {message}");

        public void LogError(Exception exception) =>
            inner.LogError($"[{context}]", exception);

        public void LogError(string message, Exception exception)
        {
            inner.LogError($"[{context}] {message}", exception);
        }
    }
}

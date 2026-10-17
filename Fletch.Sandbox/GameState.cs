namespace Fletch.Sandbox
{
    internal static class GameState
    {
        public static bool Started { get; set; } = false;

        public static bool Running { get; set; } = false;

        public static bool Ended { get; set; } = false;
    }
}

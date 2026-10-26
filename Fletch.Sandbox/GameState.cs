namespace Fletch.Sandbox
{
    internal static class GameState
    {
        public static bool Started { get; set; } = false;

        public static bool Running { get; set; } = false;

        public static bool Ended { get; set; } = false;

        public static int CurrentScore { get; set; } = 0;

        public static int BestScore { get; set; } = 0;
    }
}

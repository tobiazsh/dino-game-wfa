namespace Dino_Game_WFA.Utils
{
    public class Globals
    {
        public static readonly Font TitleFont = new Font("Arial", 24, FontStyle.Bold);
        public static readonly Font DefaultFont = new Font("Arial", 16, FontStyle.Regular);

        public static readonly string NamespaceName = "Dino_Game_WFA";

        public static readonly int GameWindowWidth = 1400;
        public static readonly int GameWindowHeight = 900;

        public static readonly float GroundY = GameWindowHeight - 200f; // Y position of the ground

        public static readonly float Scale = 1.5f; // Scale factor for game objects
    }
}

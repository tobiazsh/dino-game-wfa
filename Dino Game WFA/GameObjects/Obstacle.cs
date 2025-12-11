using Dino_Game_WFA.Resource;
using Dino_Game_WFA.Utils;

namespace Dino_Game_WFA.GameObjects
{
    public class Obstacle : GameObject<Obstacle>
    {
        public Picture TEXTURE;

        public enum ObstacleType
        {
            OBS1,
            OBS2
        }

        public Obstacle(ObstacleType type)
        {
            Type = type;
            TEXTURE = (Picture)ResourceHandler.GetResource(Identifier.Of(Globals.NamespaceName, type == ObstacleType.OBS1 ? "obstacle-1" : "obstacle-2"));
        }

        public ObstacleType Type { get; private set; }

        public bool HasBeenScored { get; set; } = false;

        public override void Draw(PaintEventArgs e)
        {
            if (TEXTURE == null)
                throw new NullReferenceException("TEXTURE for Obstacle is null!");

            Graphics graphics = e.Graphics;

            var state = graphics.Save();

            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            graphics.DrawImage(TEXTURE.Bitmap!, X, Y, Width, Height);

            graphics.Restore(state);
        }

        public void Scroll(float amount)
        {
            X -= amount;
        }

        public bool IsCompletelyOutOfBounds(float boundaryX, bool checkRightSide)
        {
            if (checkRightSide)
            {
                return X + Width < boundaryX;
            }
            else
            {
                return X > boundaryX;
            }
        }

        public bool CheckScored(float dinoX)
        {
            if (!HasBeenScored && (X + Width) < dinoX)
            {
                HasBeenScored = true;
                return true;
            }
            return false;
        }
    }
}

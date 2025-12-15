using Dino_Game_WFA.Resource;
using Dino_Game_WFA.Utils;
using System.Runtime.CompilerServices;

namespace Dino_Game_WFA.GameObjects
{
    public class Obstacle : GameObject<Obstacle>
    {
        public class Textures
        {
            public enum ObstacleType
            {
                CACTUS,
                CACTUS_MULTIPLE
            }

            public static Picture GetPicture(ObstacleType type)
            {
                return type switch
                {
                    ObstacleType.CACTUS => CACTUS,
                    ObstacleType.CACTUS_MULTIPLE => CACTUS_MULTIPLE,
                    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                };
            }

            public static readonly Picture CACTUS = (Picture) ResourceHandler.GetResource(Identifier.Of(Globals.NamespaceName, "cactus"));
            public static readonly Picture CACTUS_MULTIPLE = (Picture)ResourceHandler.GetResource(Identifier.Of(Globals.NamespaceName, "cactus_multi")); // Get Dino Texture from Resources
        }

        public readonly Picture TEXTURE;
        public readonly Textures.ObstacleType TYPE;

        public Obstacle(Textures.ObstacleType type, float X, float Y, float scale)
        {
            this.X = X;
            this.Y = Y;

            TYPE = type;
            TEXTURE = Textures.GetPicture(type);

            this.Width = TEXTURE.Bitmap!.Width * scale;
            this.Height = TEXTURE.Bitmap!.Height * scale;
        }

        public bool HasScored { get; private set; } = false;

        public void CheckScored(Dino dino, Action onScored)
        {
            if (!HasScored && dino.X > X + Width)
            {
                HasScored = true;
                onScored();
            }
        }

        /// <summary>
        /// Checks if pair is completely (inclusive the width) out of bounds
        /// </summary>
        /// <param name="boundaryX">The boundary marking whether it's out of bounds or not</param>
        /// <param name="checkLeft">Check if it's out of bounds left or right (true = left, false = right)</param>
        /// <returns></returns>
        public bool IsCompletelyOutOfBounds(float boundaryX, bool checkLeft)
        {
            if (checkLeft)
            {
                return X + Width <= boundaryX;
            }
            else
            {
                return X >= boundaryX;
            }
        }

        public override void Draw(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;

            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.DrawImage(TEXTURE.Bitmap!, X, Y, Width, Height);
        }

        public void Scroll(float amount)
        {
            X -= amount;
        }

        public static Obstacle CreateRandomObstacle(float startX, float baseY, float scale)
        {
            Array types = Enum.GetValues(typeof(Textures.ObstacleType));
            Random rand = new Random();
            Textures.ObstacleType randomType = (Textures.ObstacleType)types.GetValue(rand.Next(types.Length))!;

            return new Obstacle(
                randomType,
                startX,
                baseY - Textures.GetPicture(randomType).Bitmap!.Height * scale,
                scale);
        }
    }
}

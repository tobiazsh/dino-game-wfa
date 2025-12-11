using Flappy_Birds_WFA.Resource;
using Flappy_Birds_WFA.Utils;

namespace Flappy_Birds_WFA.GameObjects
{
    public class Pipe : GameObject<Pipe>
    {
        public static readonly Picture TEXTURE;

        static Pipe()
        {
            TEXTURE = (Picture)ResourceHandler.GetResource(Identifier.Of(Globals.NamespaceName, "pipe"));
        }

        public enum PipeType
        {
            TOP,
            BOTTOM
        }

        public Pipe(PipeType type)
        {
            Type = type;
        }

        public PipeType Type { get; private set; }

        public override void Draw(PaintEventArgs e)
        {
            if (TEXTURE == null)
                throw new NullReferenceException("TEXTURE for Pipe is null!");

            Graphics graphics = e.Graphics;

            var state = graphics.Save();

            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int rotation = Type == PipeType.TOP ? 180 : 0;

            // Only rotate if necessary
            if (rotation == 0 || rotation % 360 != 0)
            {
                graphics.TranslateTransform((float)(X + Width / 2), (float)(Y + Height / 2)); // Move to center of Pipe
                graphics.RotateTransform(rotation); // Rotate
                graphics.TranslateTransform(-(float)(X + Width / 2), -(float)(Y + Height / 2)); // Move back
            }

            graphics.DrawImage(TEXTURE.Bitmap!, X, Y, Width, Height);

            graphics.Restore(state);
        }

        public void Scroll(float amount)
        {
            X -= amount;
        }
    }

    public class PipePair
    {
        public Pipe TopPipe { get; init; }
        public Pipe BottomPipe { get; init; }
        public EmptyGameObject ScoreCheck { get; init; }
        public bool HasBeenScored { get; set; } = false;

        public PipePair(float topPipeHeight, float bottomPipeHeight, float availableHeight, float xPosition, float pipeWidth)
        {
            TopPipe = new Pipe(Pipe.PipeType.TOP)
                .SetPosition(xPosition, 0)
                .SetBounds(pipeWidth, topPipeHeight); ;

            BottomPipe = new Pipe(Pipe.PipeType.BOTTOM)
                .SetPosition(xPosition, availableHeight - bottomPipeHeight)
                .SetBounds(pipeWidth, bottomPipeHeight); ;
             
            ScoreCheck = new EmptyGameObject()
                .SetPosition(xPosition + pipeWidth, 0)
                .SetBounds(1, availableHeight); // 1 pixel wide score check
        }

        public static PipePair GenerateRandom(float minGapHeight, float maxGapHeight, float minPipeWidth, float maxPipeWidth, float availableHeight, float x)
        {
            Random random = new Random();
            float gapHeight = (float)random.NextDouble() * (maxGapHeight - minGapHeight) + minGapHeight; // Generate Random Gap Height
            float pipeWidth = (float)random.NextDouble() * (maxPipeWidth - minPipeWidth) + minPipeWidth; // Generate Random Pipe Width
            float topPipeHeight = (float)random.NextDouble() * (availableHeight - gapHeight); // Random Top Pipe Height

            return new PipePair(
                topPipeHeight,
                availableHeight - gapHeight - topPipeHeight,
                availableHeight,
                x,
                pipeWidth
            );
        }

        public void Scroll(float amount)
        {
            TopPipe.Scroll(amount);
            BottomPipe.Scroll(amount);
            ScoreCheck.X -= amount;
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
                return TopPipe.X + TopPipe.Width <= boundaryX;
            }
            else
            {
                return TopPipe.X >= boundaryX;
            }
        }

        public void Draw(PaintEventArgs e)
        {
            TopPipe.Draw(e);
            BottomPipe.Draw(e);
        }

        public bool IntersectsWith(GameObject other)
        {
            return TopPipe.IntersectsWith(other) || BottomPipe.IntersectsWith(other);
        }

        public bool CheckScore(GameObject other)
        {
            return ScoreCheck.IntersectsWith(other);
        }
    }
}

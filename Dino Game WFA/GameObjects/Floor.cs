using Flappy_Birds_WFA.Resource;
using Flappy_Birds_WFA.Utils;

namespace Flappy_Birds_WFA.GameObjects
{
    public class Floor : GameObject<Floor>
    {
        public static readonly Picture TEXTURE;

        static Floor()
        {
            TEXTURE = (Picture)ResourceHandler.GetResource(Identifier.Of(Globals.NamespaceName, "ground"));
        }

        // Implmented from GameObject
        public override void Draw(PaintEventArgs e)
        {
            if (TEXTURE == null)
                throw new NullReferenceException("TEXTURE for Floor is null!");

            Graphics paintGraphics = e.Graphics;

            var state = paintGraphics.Save();

            paintGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            paintGraphics.DrawImage(TEXTURE.Bitmap!, X, Y, Width, Height);

            paintGraphics.Restore(state);
        }

        public void Scroll(float amount)
        {
            X -= amount;
        }
    }
}

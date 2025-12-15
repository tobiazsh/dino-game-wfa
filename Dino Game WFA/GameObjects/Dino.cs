using Dino_Game_WFA.Resource;
using Dino_Game_WFA.Utils;

namespace Dino_Game_WFA.GameObjects
{
    public class Dino : GameObject<Dino>
    {
        public static readonly Picture TEXTURE_DEAD = (Picture) ResourceHandler.GetResource(Identifier.Of(Globals.NamespaceName, "dead"));
        public static readonly Picture TEXTURE_RUNNING = (Picture)ResourceHandler.GetResource(Identifier.Of(Globals.NamespaceName, "running")); // Get Dino Texture from Resources
        public Picture texture;

        public Dino(float startingPositionY, float scale)
        {
            Y = startingPositionY;
            _scale = scale;
            _STARTING_Y = startingPositionY;
            texture = TEXTURE_RUNNING; // Set initial texture to running
            TextureChange(TEXTURE_RUNNING);
        }

        private readonly float _STARTING_Y;

        public float _velocity = 0f; // pixels / second

        private readonly float _scale;

        public const float GRAVITY = 2000f;         // pixels / second^2
        public const float JUMP_VELOCITY = -525f;   // initial jump impulse (pixels / second)
        public const float MAX_FALL_SPEED = 1200f;  // terminal velocity (pixels / second)
        public const float ROTATION_UP = -25f;      // degrees when ascending
        public const float ROTATION_DOWN = 85f;     // degrees when descending
        public const float ROTATION_SPEED = 300f;   // degrees per second

        /// <summary>
        /// Causes the Dino to Jump.
        /// </summary>
        /// <param name="allowMultiple">If false, Dino will wait until it's at startingPositionY again OR lower to be able to jump again, or in other words, until the last jump is finished.</param>
        public void Jump(bool allowMultiple)
        {
            if (!allowMultiple && this.Y < _STARTING_Y)
                return; // Do not allow multiple jumps

            _velocity = JUMP_VELOCITY * _scale;
        }

        /// <summary>
        /// Calculate Dino Position
        /// </summary>
        /// <param name="floorY">The min height of the dino's position</param>
        /// <param name="ceilY">The max height of the dino's position</param>
        /// <param name="dt">Delta Time since last frame in seconds</param>
        public void Calculate(float floorY, float ceilY, float dt)
        {
            // Animate Falling/Jumping
            if (dt <= 0f) return; // No time has passed

            _velocity += GRAVITY * dt; // Apply gravity
            if (_velocity > MAX_FALL_SPEED)
                _velocity = MAX_FALL_SPEED; // Cap fall speed

            Y += _velocity * dt; // Update position

            // Clamp to bounds and zero velocity on contact
            if (Y > floorY)
            {
                Y = floorY;
                _velocity = 0f;
            }
            if (Y < ceilY)
            {
                Y = ceilY;
                _velocity = 0f;
            }
        }

        // Implemented from GameObject
        public override void Draw(PaintEventArgs e)
        {
            if (texture == null)
                throw new NullReferenceException("TEXTURE for Dino is null!");
            Graphics paintGraphics = e.Graphics;

            var state = paintGraphics.Save();

            paintGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Draw Dino
            paintGraphics.DrawImage(texture.Bitmap!, X, Y, Width, Height); // Draw Bird :=)

            paintGraphics.Restore(state);
        }

        private void TextureChange(Picture texture)
        {
            this.texture = texture;
            this.Width = texture.Bitmap!.Width * _scale;
            this.Height = texture.Bitmap!.Height * _scale;
        }

        public void Deadify()
        {
            TextureChange(TEXTURE_DEAD); // Get Dead Bird Texture from Resources
        }
    }
}

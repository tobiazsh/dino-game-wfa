using Dino_Game_WFA.Resource;
using Dino_Game_WFA.Utils;

namespace Dino_Game_WFA.GameObjects
{
    public class Dino : GameObject<Dino>
    {
        public static readonly Picture TEXTURE_DEAD = (Picture) ResourceHandler.GetResource(Identifier.Of(Globals.NamespaceName, "dead"));
        public static readonly Picture TEXTURE_RUNNING = (Picture)ResourceHandler.GetResource(Identifier.Of(Globals.NamespaceName, "running")); // Get Dino Texture from Resources
        public Picture TEXTURE;

        public Dino(float startingPositionY)
        {
            Y = startingPositionY;
            TEXTURE = TEXTURE_RUNNING; // Set initial texture to running
        }

        public float _velocity = 0f; // pixels / second
        public float Rotation { get; private set; } = 0f; // degrees

        public const float GRAVITY = 2000f;         // pixels / second^2
        public const float JUMP_VELOCITY = -450f;   // initial jump impulse (pixels / second)
        public const float MAX_FALL_SPEED = 1200f;   // terminal velocity (pixels / second)
        public const float ROTATION_UP = -25f;      // degrees when ascending
        public const float ROTATION_DOWN = 85f;     // degrees when descending
        public const float ROTATION_SPEED = 300f;   // degrees per second

        public void Jump()
        {
            _velocity = JUMP_VELOCITY;
        }

        /// <summary>
        /// Calculate Bird Position
        /// </summary>
        /// <param name="minHeight">The min height of the bird's position</param>
        /// <param name="maxHeight">The max height of the bird's position</param>
        /// <param name="dt">Delta Time since last frame in seconds</param>
        public void Calculate(float minHeight, float maxHeight, float dt)
        {
            // Animate Falling/Jumping
            if (dt <= 0f) return; // No time has passed

            _velocity += GRAVITY * dt; // Apply gravity
            if (_velocity > MAX_FALL_SPEED)
                _velocity = MAX_FALL_SPEED; // Cap fall speed

            Y += _velocity * dt; // Update position

            // Clamp to bounds and zero velocity on contact
            if (Y > minHeight)
            {
                Y = minHeight;
                _velocity = 0f;
            }
            if (Y < maxHeight)
            {
                Y = maxHeight;
                _velocity = 0f;
            }

            // Compute target rotation based on velocity
            // velocity is negative when going up, positive when going down
            float vMin = JUMP_VELOCITY;  // most negative velocity
            float vMax = MAX_FALL_SPEED; // most positive velocity
            float t = (_velocity - vMin) / (vMax - vMin); // normalized [0, 1]
            t = Math.Clamp(t, 0f, 1f);
            float targetRotation = ROTATION_UP + t * (ROTATION_DOWN - ROTATION_UP);

            // Smoothly move rotation toward target
            float maxDelta = ROTATION_SPEED * dt;
            float diff = targetRotation - Rotation;
            if (Math.Abs(diff) <= maxDelta)
                Rotation = targetRotation;
            else
                Rotation += Math.Sign(diff) * maxDelta;
        }

        // Implemented from GameObject
        public override void Draw(PaintEventArgs e)
        {
            if (TEXTURE == null)
                throw new NullReferenceException("TEXTURE for Bird is null!");
            Graphics paintGraphics = e.Graphics;

            var state = paintGraphics.Save();

            paintGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Rotate Bird
            paintGraphics.TranslateTransform((float)(X + Width / 2), (float)(Y + Height / 2)); // Move to center of Bird
            paintGraphics.RotateTransform((float)Rotation); // Rotate Bird
            paintGraphics.TranslateTransform(-(float)(X + Width / 2), -(float)(Y + Height / 2)); // Move back

            // Draw Bird
            paintGraphics.DrawImage(TEXTURE.Bitmap!, X, Y, Width, Height); // Draw Bird :=)

            paintGraphics.Restore(state);
        }

        public void Deadify()
        {
            TEXTURE = TEXTURE_DEAD; // Get Dead Bird Texture from Resources
        }
    }
}

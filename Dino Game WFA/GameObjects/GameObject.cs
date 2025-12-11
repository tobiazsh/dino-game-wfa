namespace Flappy_Birds_WFA.GameObjects
{
    public abstract class GameObject
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public bool IntersectsWith(GameObject other)
        {
            return X + Width > other.X &&
                    X < other.X + other.Width &&
                    Y + Height > other.Y &&
                    Y < other.Y + other.Height;
        }

        public abstract void Draw(PaintEventArgs e);
    }

    public abstract class GameObject<T> : GameObject
        where T : GameObject<T>
    {
        public T SetPosition(float x, float y)
        {
            X = x;
            Y = y;
            return (T)this;
        }

        public T SetBounds(float width, float height)
        {
            Width = width;
            Height = height;
            return (T)this;
        }
    }
}

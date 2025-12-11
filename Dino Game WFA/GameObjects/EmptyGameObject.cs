namespace Flappy_Birds_WFA.GameObjects
{
    public class EmptyGameObject : GameObject<EmptyGameObject>
    {
        public override void Draw(PaintEventArgs e)
        {
            return; // Do nothing
        }
    }
}

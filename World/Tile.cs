using System.Numerics;
using Raylib_cs;

public class Tile
{
    public Vector2 Position { get; private set; }
    public int Layer { get; private set; }
    public int Depth { get; private set; }
    public Sprite? Sprite { get; private set; }
    public Rectangle WorldBoundingBox { get; private set; }

    public Tile(Vector2 position, int layer, int depth, Sprite? sprite)
    {
        Position = position;
        Layer = layer;
        Depth = depth;
        WorldBoundingBox = new Rectangle((int)position.X, (int)position.Y, 1, 1).GridToWorld();
        Sprite = sprite;
    }

    public void Draw()
    {
        // If sprite is null, then just return
        if (Sprite == null) return;
        // Otherwise draw the sprite at the position
        Sprite.Draw(Position);
    }
}
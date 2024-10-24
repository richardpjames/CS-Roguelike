using System.Numerics;
using Raylib_cs;

public class Tile
{
    public Vector4 Position { get; private set; }
    public Sprite? Sprite { get; private set; }
    public Rectangle BoundingBox { get; private set; }

    public Tile(Vector4 position, Sprite? sprite)
    {
        Position = position;
        BoundingBox = new Rectangle((int)position.X, (int)position.Y, Game.PIXELS_PER_UNIT, Game.PIXELS_PER_UNIT);
        Sprite = sprite;
    }

    public void Draw()
    {
        // If sprite is null, then just return
        if (Sprite == null) return;
        // Otherwise draw the sprite at the position
        Sprite.Draw(new Vector2(Position.X, Position.Y));
    }
}
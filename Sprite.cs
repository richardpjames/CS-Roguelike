using Raylib_cs;
using System.Numerics;

public class Sprite
{
    private Texture2D _texture;
    private Rectangle _drawRectangle;
    public Sprite(string fileName, Rectangle spriteSheetPosition)
    {
        // This is our rectangle for defining the sprite within the spritesheet
        _drawRectangle = spriteSheetPosition;
        // Load the specified texture from the file
        _texture = Raylib.LoadTexture(fileName);
    }

    public void Draw(Vector2 position)
    {
        // Draw the texture at the requested position and adjust the center position based on the draw rectangle
        Raylib.DrawTextureRec(_texture, _drawRectangle, new Vector2(position.X, position.Y), Color.White);
    }
}


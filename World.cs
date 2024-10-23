using ldtk;
using System.Numerics;
using System.IO;
using Raylib_cs;

public class World : GameObject
{
    private LdtkJson _worldJson;
    private Texture2D _texture;

    public World(string fileName) : base(new Vector2(0, 0))
    {
        // Load the json string from the specified file
        string jsonData = File.ReadAllText(fileName);
        // Run through the ldtk parser
        _worldJson = LdtkJson.FromJson(jsonData);
        // Load the texture once in the constructor
        _texture = Raylib.LoadTexture("Assets/tiles.png");
    }

    public override void Update(float deltaTime)
    {
        // There are currently no updates to perform on the world
        return;
    }

    public override void Render(Rectangle cameraBounds)
    {
        // Loop through each level
        foreach (Level level in _worldJson.Levels)
        {
            // Find the area that the level covers and only go further if rendered
            Rectangle levelRectangle = new Rectangle(level.WorldX, level.WorldY, level.PxWid, level.PxHei);
            // Then each layer within the level
            if (Raylib.CheckCollisionRecs(cameraBounds, levelRectangle))
            {
                // Go through each layer in reverse order to render lower layers first
                foreach (LayerInstance layer in level.LayerInstances.Reverse())
                {
                    // Then each tile within the layer
                    foreach (TileInstance tile in layer.GridTiles)
                    {
                        Rectangle sourceRectangle = new Rectangle(tile.Src[0], tile.Src[1], 32, 32);
                        // Offset for the world location on each level
                        Rectangle destinationRectangle = new Rectangle(level.WorldX + tile.Px[0], level.WorldY + tile.Px[1], 32, 32);
                        // Check whether this tile is within the camera bounds in order to render it
                        if (Raylib.CheckCollisionRecs(cameraBounds, destinationRectangle))
                        {
                            // Determine the position based on the destination
                            Raylib.DrawTextureRec(_texture, sourceRectangle, new Vector2(destinationRectangle.X, destinationRectangle.Y), Color.White);
                        }
                    }
                }
            }
        }
    }
}


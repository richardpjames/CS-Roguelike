using ldtk;
using System.Numerics;
using System.IO;
using Raylib_cs;

public class World : GameObject
{
    private LdtkJson _worldJson;
    private Texture2D _texture;
    // Holds tiles for rendering
    private Dictionary<Vector4, Tile> _tiles;
    // Holds rectangles which represent collision shapes
    private List<Rectangle> _collisions;

    public World(string fileName) : base(new Vector2(0, 0))
    {
        // Initialise the dictionary of tiles
        _tiles = new Dictionary<Vector4, Tile>();
        _collisions = new List<Rectangle>();
        // Load the json string from the specified file
        string jsonData = File.ReadAllText(fileName);
        // Run through the ldtk parser
        _worldJson = LdtkJson.FromJson(jsonData);
        // Load the texture once in the constructor
        _texture = Raylib.LoadTexture("Assets/tiles.png");
        // Create all of the tiles from the ldtk file provided
        int layerNumber = 0;
        // Loop through each level
        foreach (Level level in _worldJson.Levels)
        {
            // Go through each layer in reverse order to render lower layers first
            foreach (LayerInstance layer in level.LayerInstances.Reverse())
            {
                // Then each tile within the layer
                foreach (TileInstance tile in layer.GridTiles)
                {
                        // Where in the texture do we take the sprite from
                        Rectangle sourceRectangle = new Rectangle(tile.Src[0], tile.Src[1], 32, 32);
                        // Offset for the world location on each level - store the layer and depth and the third and fourth values
                        Vector4 position = new Vector4(level.WorldX + tile.Px[0], level.WorldY + tile.Px[1], layerNumber, level.WorldDepth);
                        // Create a sprite from the texture and the source rectangle
                        Sprite tileSprite = new Sprite(_texture, sourceRectangle);
                        // Create the tile with the position and sprite
                        Tile tileObject = new Tile(position, tileSprite);
                        // Add that tile to the list of tiles
                        if (!_tiles.ContainsKey(position)) _tiles.Add(position, tileObject);
                    
                }
                // For each entity within the layer
                foreach (EntityInstance entity in layer.EntityInstances)
                {
                    // Find any colliders and add to the lits
                    if(entity.Identifier == "Collider")
                    {
                        Rectangle collider = new Rectangle(level.WorldX + entity.Px[0], level.WorldY + entity.Px[1], entity.Width, entity.Height);
                        _collisions.Add(collider);
                    }
                }
                // Keeps track of the layer ordering for tiles
                layerNumber++;
            }
        }
    }

    public bool DetectWorldCollisions(Rectangle rectangle)
    {
        // Check all tiles for collisions
        foreach (Rectangle collisionRectangle in _collisions)
        {
            // The tile must not be walkable and must directly collide
            if (Raylib.CheckCollisionRecs(rectangle, collisionRectangle))
            {
                // Add to the selected tiles
                return true;
            }
        }
        // We have not collided if we reach here
        return false;
    }

    public override void Update(float deltaTime)
    {
        // There are currently no updates to perform on the world
        return;
    }

    public override void Render(Rectangle cameraBounds)
    {
        // Loop through all tiles
        foreach (Tile tile in _tiles.Values)
        {
            // Determine whether to draw the tile based on whether it falls within the camera view
            if (Raylib.CheckCollisionRecs(cameraBounds, tile.BoundingBox))
            {
                // Draw them to the screen
                tile.Draw();
            }
        }
    }
}


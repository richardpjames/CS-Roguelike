using ldtk;
using System.Numerics;
using Raylib_cs;

public class World : GameObject
{
    private LdtkJson _worldJson;
    private Texture2D _texture;
    // Holds tiles for rendering
    private Dictionary<Vector4, Tile> _tiles;
    // Holds rectangles which represent collision shapes
    private List<Vector3> _collisions;
    // Holds integers at positions to set depth
    private Dictionary<Vector3, int> _stairs;
    // Holds the current world depth
    private int currentDepth = 0;

    public World(string fileName) : base(new Vector2(0, 0))
    {
        // Initialise the dictionary of tiles
        _tiles = new Dictionary<Vector4, Tile>();
        _collisions = new List<Vector3>();
        _stairs = new Dictionary<Vector3, int>();
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
                    Rectangle sourceRectangle = new Rectangle(tile.Src[0], tile.Src[1], Game.PIXELS_PER_UNIT,
                        Game.PIXELS_PER_UNIT);
                    // Offset for the world location on each level - store the layer and depth and the third and fourth values
                    Vector2 position = new Vector2(level.WorldX + tile.Px[0], level.WorldY + tile.Px[1]).WorldToGrid();
                    // Create a sprite from the texture and the source rectangle
                    Sprite tileSprite = new Sprite(_texture, sourceRectangle);
                    // Create the tile with the position and sprite
                    Tile tileObject = new Tile(position, layerNumber, (int)level.WorldDepth, tileSprite);
                    // Add that tile to the list of tiles
                    Vector4 key = new Vector4(tileObject.Position.X, tileObject.Position.Y, tileObject.Layer, tileObject.Depth);
                    if (!_tiles.ContainsKey(key)) _tiles.Add(key, tileObject);
                }

                // For each entity within the layer
                foreach (EntityInstance entity in layer.EntityInstances)
                {
                    // Find any colliders and add to the lits
                    if (entity.Identifier == "Collider")
                    {
                        Vector2 position = new Vector2(level.WorldX + entity.Px[0], level.WorldY + entity.Px[1]).WorldToGrid();
                        Vector3 collider = new Vector3(position.X, position.Y, (int)level.WorldDepth);
                        _collisions.Add(collider);
                    }
                    // Find any colliders and add to the lits
                    if (entity.Identifier == "DepthChange")
                    {
                        Vector2 position = new Vector2(level.WorldX + entity.Px[0], level.WorldY + entity.Px[1]).WorldToGrid();
                        _stairs.Add(new Vector3(position.X, position.Y, (int)level.WorldDepth), (int)entity.FieldInstances[0].Value);
                    }
                }
                // Keeps track of the layer ordering for tiles
                layerNumber++;
            }
        }
    }

    public bool CheckCollision(Vector2 position, int depth)
    {
        // Check that a tile (on any layer) exists for this position
        if (_tiles.Values.FirstOrDefault<Tile>((tile) => { return tile.Position.X == position.X && tile.Position.Y == position.Y; }) != null)
        {
            // If so, check whether there are any collision tiles
            return _collisions.Contains(new Vector3(position.X, position.Y, depth));
        }
        // We will reach here if there is no tile on the map for that position
        return true;
    }

    public int ChangeLevel(Vector2 position, int depth)
    {
        Vector3 key = new Vector3(position.X, position.Y, depth);
        // If there are stairs, then we adjust by the value
        if (_stairs.ContainsKey(key))
        {
            currentDepth += _stairs[key];
        }
        // return the new depth
        return currentDepth;
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
            if (Raylib.CheckCollisionRecs(cameraBounds, tile.WorldBoundingBox) && tile.Depth == currentDepth)
            {
                // Draw them to the screen
                tile.Draw();
            }
        }
    }
}
using ldtk;
using System.Numerics;
using Raylib_cs;

public class World : GameObject
{
    // Holds the JSON representation of the world
    private LdtkJson _worldJson;
    // Needed to add monsters from the LDTK definition
    private MonsterManager? _monsterManager;
    // Used for loading the world textures
    private Texture2D _texture;
    // Used for creating the monsters
    private Texture2D _monsterTexture;
    // Holds tiles for rendering
    private Dictionary<Vector4, Tile> _tiles;
    // Holds rectangles which represent collision shapes
    private List<Vector3> _collisions;
    // Holds integers at positions to set depth
    private Dictionary<Vector3, int> _stairs;
    // Holds the current world depth
    private int currentDepth = 0;

    public World(string fileName, MonsterManager monsterManager) : base(new Vector2(0, 0))
    {
        // Initialise the dictionary of tiles
        _tiles = new Dictionary<Vector4, Tile>();
        _collisions = new List<Vector3>();
        _stairs = new Dictionary<Vector3, int>();
        // Set the reference to the monster manager
        _monsterManager = monsterManager;
        // Load the json string from the specified file
        string jsonData = File.ReadAllText(fileName);
        // Run through the ldtk parser
        _worldJson = LdtkJson.FromJson(jsonData);
        // Load the texture once in the constructor
        _texture = Raylib.LoadTexture("Assets/tiles.png");
        _monsterTexture = Raylib.LoadTexture("Assets/monsters.png");
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
                    Vector2 position = new Vector2(level.WorldX + entity.Px[0], level.WorldY + entity.Px[1]).WorldToGrid();
                    // Find any colliders and add to the lits
                    if (entity.Identifier == "Collider")
                    {
                        Vector3 collider = new Vector3(position.X, position.Y, (int)level.WorldDepth);
                        _collisions.Add(collider);
                    }
                    // Find any colliders and add to the lits
                    if (entity.Identifier == "DepthChange")
                    {
                        _stairs.Add(new Vector3(position.X, position.Y, (int)level.WorldDepth), (int)entity.FieldInstances[0].Value);
                    }
                    // Find any monsters to load and add to the monster manager
                    if (entity.Identifier == "Monster")
                    {
                        // Pull the sprite grid position
                        Vector2 spritePosition = new Vector2((int)entity.FieldInstances[0].Value, (int)entity.FieldInstances[1].Value).GridToWorld();
                        // Get the sprite based on the provided grid position above
                        Sprite monsterSprite = new Sprite(_monsterTexture, new Rectangle(spritePosition.X, spritePosition.Y, Game.PIXELS_PER_UNIT, Game.PIXELS_PER_UNIT));
                        // Create the monster and add it to the manager
                        Monster monster = new Monster(position, monsterSprite, this, (int)level.WorldDepth, _monsterManager, (float)entity.FieldInstances[2].Value, (int)entity.FieldInstances[3].Value);
                    }
                }
                // Keeps track of the layer ordering for tiles
                layerNumber++;
            }
        }
    }

    // Unloac textures on destruction of the world
    ~World()
    {
        Raylib.UnloadTexture(_texture);
        Raylib.UnloadTexture(_monsterTexture);
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
        // Update the mosnter manager
        if (_monsterManager != null)
        {
            _monsterManager.CurrentDepth = currentDepth;
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
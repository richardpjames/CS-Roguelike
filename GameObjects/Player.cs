using System.Numerics;
using Raylib_cs;

public class Player : GameObject
{
    public Sprite Sprite { get; private set; }
    private const float SPEED = 10;
    private World? _world;
    private int currentDepth = 0;

    public Player(Vector2 position, Sprite sprite, World world) : base(position)
    {
        // The position is set by the base constructor - set the sprite for the player
        this.Sprite = sprite;
        this._world = world;
    }

    public override void Update(float deltaTime)
    {
        if (_world == null) return;
        // Check for interaction events
        if (Raylib.IsKeyReleased(KeyboardKey.E))
        {
            currentDepth = _world.ChangeLevel(Position, currentDepth);
        }
        // Otherwise allow movement
        else
        {
            // Get the input from the player
            Vector2 inputVector = GetInputVector();
            // If there is no input then return
            if (inputVector == Vector2.Zero) return;
            // Check for collisions in the new location
            if (!_world.CheckCollision(Position + inputVector, currentDepth))
            {
                Position = Position + inputVector;
            }
        }
    }

    private Vector2 GetInputVector()
    {
        // Initialise the vector
        Vector2 direction = new Vector2(0, 0);
        // Up movement
        if (Raylib.IsKeyReleased(KeyboardKey.W) || Raylib.IsKeyReleased(KeyboardKey.Up))
        {
            direction.Y -= 1;
        }
        // Left movement
        else if (Raylib.IsKeyReleased(KeyboardKey.A) || Raylib.IsKeyReleased(KeyboardKey.Left))
        {
            direction.X -= 1;
        }
        // Down movement
        else if (Raylib.IsKeyReleased(KeyboardKey.S) || Raylib.IsKeyReleased(KeyboardKey.Down))
        {
            direction.Y += 1;
        }
        // Right movement
        else if (Raylib.IsKeyReleased(KeyboardKey.D) || Raylib.IsKeyReleased(KeyboardKey.Right))
        {
            direction.X += 1;
        }
        // Return the direction
        return direction;

    }
    public override void Render(Rectangle cameraBounds)
    {
        // Draw the sprite at our player position
        Sprite.Draw(Position);
    }
}

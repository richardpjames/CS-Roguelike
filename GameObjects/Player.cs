using System.Numerics;
using Raylib_cs;

public class Player : GameObject
{
    public Sprite Sprite { get; private set; }
    private const float SPEED = 200;
    private World? _world;

    public Player(Vector2 position, Sprite sprite, World world) : base(position)
    {
        // The position is set by the base constructor - set the sprite for the player
        this.Sprite = sprite;
        this._world = world;
    }

    public override void Update(float deltaTime)
    {
        if (_world == null) return;
        // Get the input from the player
        Vector2 inputVector = GetInputVector();
        // If there is no input then return
        if (inputVector == Vector2.Zero) return;
        // Update based on the input for the player (vector for direction)
        Position.X += inputVector.X * SPEED * deltaTime;
        // If so then reset the X position
        if (_world.DetectWorldCollisions(new Rectangle(Position.X + 4, Position.Y + 1, 21, 30)))
        {
            Position.X -= inputVector.X * SPEED * deltaTime;
        }
        // Update based on the input for the player (vector for direction)
        Position.Y += inputVector.Y * SPEED * deltaTime;
        // If so then reset the X position
        if (_world.DetectWorldCollisions(new Rectangle(Position.X + 4, Position.Y + 1, 21, 30)))
        {
            Position.Y -= inputVector.Y * SPEED * deltaTime;
        }
    }

    private Vector2 GetInputVector()
    {
        // Initialise the vector
        Vector2 direction = new Vector2(0, 0);
        // Up movement
        if (Raylib.IsKeyDown(KeyboardKey.W) || Raylib.IsKeyDown(KeyboardKey.Up))
        {
            direction.Y -= 1;
        }
        // Left movement
        if (Raylib.IsKeyDown(KeyboardKey.A) || Raylib.IsKeyDown(KeyboardKey.Left))
        {
            direction.X -= 1;
        }
        // Down movement
        if (Raylib.IsKeyDown(KeyboardKey.S) || Raylib.IsKeyDown(KeyboardKey.Down))
        {
            direction.Y += 1;
        }
        // Right movement
        if (Raylib.IsKeyDown(KeyboardKey.D) || Raylib.IsKeyDown(KeyboardKey.Right))
        {
            direction.X += 1;
        }
        // If all is zero then return without normalising
        if (direction.X + direction.Y == 0) return direction;
        // Return the direction as a vector (normalised to avoid quick diagonals)
        return Vector2.Normalize(direction);
    }
    public override void Render(Rectangle cameraBounds)
    {
        // Draw the sprite at our player position
        Sprite.Draw(Position);
    }
}

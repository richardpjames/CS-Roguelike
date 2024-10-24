using System.Numerics;
using Raylib_cs;

public class Player : GameObject
{
    public Sprite Sprite { get; private set; }
    private const float SPEED = 10;
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
        Position.X += inputVector.X;
        // If so then reset the X position
        if (_world.DetectWorldCollisions(new Rectangle((Position.X * Game.PIXELS_PER_UNIT) + 4, (Position.Y * Game.PIXELS_PER_UNIT) + 1, 21, 30)))
        {
            Position.X -= inputVector.X;
        }
        // Update based on the input for the player (vector for direction)
        Position.Y += inputVector.Y;
        // If so then reset the X position
        if (_world.DetectWorldCollisions(new Rectangle((Position.X * Game.PIXELS_PER_UNIT) + 4, (Position.Y * Game.PIXELS_PER_UNIT) + 1, 21, 30)))
        {
            Position.Y -= inputVector.Y;
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

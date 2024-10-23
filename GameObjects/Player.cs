using System.Numerics;
using Raylib_cs;

public class Player : GameObject
{
    public Sprite Sprite { get; private set; }
    private const float SPEED = 200;
    public Player(Vector2 position, Sprite sprite) : base(position)
    {
        // The position is set by the base constructor - set the sprite for the player
        this.Sprite = sprite;
    }

    public override void Update(float deltaTime)
    {
        Vector2 inputVector = GetInputVector();
        // Update based on the input for the player (vector for direction)
        Position += inputVector * SPEED * deltaTime;
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

using System.Numerics;
using Raylib_cs;
using Unglide;

public class Player : GameObject
{
    public Sprite Sprite { get; private set; }
    private const float SPEED = 10;
    private World? _world;
    private int currentDepth = 0;
    // Must ensure that the audio is initialised before loading sounds!
    private Sound _footsteps;
    private Sound _stairs;
    private Vector2 targetPosition;
    // Actions to signal back to the game
    public Action? OnPlayerMoved;
    public Action? OnInputProcessed;
    // For tweening movement and attacks
    private Tweener? _movementTween;
    // For setting and getting the X,Y positions for this object more easily
    public float X { get { return Position.X; } set { Position.X = value; } }
    public float Y { get { return Position.Y; } set { Position.Y = value; } }


    public Player(Vector2 position, Sprite sprite, World world) : base(position)
    {
        // The position is set by the base constructor - set the sprite for the player
        this.Sprite = sprite;
        this._world = world;
        // Load the required sounds
        _footsteps = Raylib.LoadSound("Assets/sound/16_human_walk_stone_1.wav");
        _stairs = Raylib.LoadSound("Assets/sound/05_door_open_1.mp3");
        _movementTween = new Tweener();
    }

    ~Player()
    {
        // Unload sounds when the player is destroyed
        Raylib.UnloadSound(_footsteps);
        Raylib.UnloadSound(_stairs);
    }

    public override void Update(float deltaTime)
    {
        // Update the tweener each frame
        if (_movementTween != null)
        {
            _movementTween.Update(deltaTime);
        }
    }

    public void ProcessInput()
    {
        if (_world == null) return;
        // Check for interaction events
        if (Raylib.IsKeyReleased(KeyboardKey.E))
        {
            int newDepth = _world.ChangeLevel(Position, currentDepth);
            if (newDepth != currentDepth)
            {
                // Update the current depth for the player and confirm movement
                currentDepth = newDepth;
                Raylib.PlaySound(_stairs);
                // Movement is instantaneous for stairs, so simply signal the move is complete
                OnPlayerMoved?.Invoke();
            }
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
                // TODO - Check for enemies and provoke an attack
                targetPosition = Position + inputVector;
                Raylib.PlaySound(_footsteps);
                // Let the game know that we have processed the input - this will cause update to run
                OnInputProcessed?.Invoke();
                // This will move the player to their new position over xx seconds with 0 delay
                if (_movementTween != null)
                {
                    _movementTween.Tween(this, new { X = targetPosition.X, Y = targetPosition.Y }, 0.15f, 0)
                        .Ease(Ease.CubeInOut)
                        // On completion we need to confirm to the game that the player has moved
                        .OnComplete(() => { OnPlayerMoved?.Invoke(); });
                }
                // On attacking - add a repeat(1) and reflect() call to the above
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

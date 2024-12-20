﻿using System.Numerics;
using Raylib_cs;
using Unglide;

public class Player : GameObject
{
    public Sprite Sprite { get; private set; }
    private World? _world;
    private MonsterManager? _monsterManager;
    private int currentDepth = 0;
    // Must ensure that the audio is initialised before loading sounds!
    private Sound _footsteps;
    private Sound _attack;
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


    public Player(Vector2 position, Sprite sprite, World world, MonsterManager monsterManager) : base(position)
    {
        // The position is set by the base constructor - set the sprite for the player
        Sprite = sprite;
        _world = world;
        _monsterManager = monsterManager;
        // Load the required sounds
        _footsteps = Raylib.LoadSound("Assets/sound/16_human_walk_stone_1.wav");
        _attack = Raylib.LoadSound("Assets/sound/07_human_atk_sword_1.wav");
        _stairs = Raylib.LoadSound("Assets/sound/05_door_open_1.mp3");
        _movementTween = new Tweener();
    }

    ~Player()
    {
        // Unload sounds when the player is destroyed
        Raylib.UnloadSound(_attack);
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
            // First check for monsters and attack if present
            Monster? monster = _monsterManager?.GetMonster(Position + inputVector, currentDepth);
            if (_monsterManager != null && monster != null)
            {
                // Take damage
                bool monsterKilled = monster.TakeDamage(1);
                targetPosition = Position + inputVector;
                Raylib.PlaySound(_attack);
                // Force the monster to take damage
                // Let the game know that we have processed the input - this will cause update to run
                OnInputProcessed?.Invoke();
                // This will move the player to their new position and back again over xx seconds with 0 delay
                if (_movementTween != null)
                {

                    _movementTween.Tween(this, new { X = targetPosition.X, Y = targetPosition.Y }, 0.15f, 0)
                        .Ease(Ease.CubeInOut)
                        // If the monster is killed then don't repeat - otherwise repeat once
                        .Repeat(monsterKilled ? 0 : 1)
                        .Reflect()
                        // On completion we need to confirm to the game that the player has moved
                        .OnComplete(() => { OnPlayerMoved?.Invoke(); });
                }
            }
            // If no monsters then check for collisions in the new location
            else if (!_world.CheckCollision(Position + inputVector, currentDepth))
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
            }
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
        else if (Raylib.IsKeyDown(KeyboardKey.A) || Raylib.IsKeyDown(KeyboardKey.Left))
        {
            direction.X -= 1;
        }
        // Down movement
        else if (Raylib.IsKeyDown(KeyboardKey.S) || Raylib.IsKeyDown(KeyboardKey.Down))
        {
            direction.Y += 1;
        }
        // Right movement
        else if (Raylib.IsKeyDown(KeyboardKey.D) || Raylib.IsKeyDown(KeyboardKey.Right))
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

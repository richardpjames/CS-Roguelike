using Raylib_cs;
using System.Numerics;

public class Monster : GameObject
{
    public Sprite Sprite { get; private set; }
    private World? _world;
    public int CurrentDepth;
    private MonsterManager _monsterManager;
    private float _health;
    private float _strength;
    private Sound _deathSound;

    public Monster(Vector2 position, Sprite sprite, World world, int currentDepth, MonsterManager monsterManager, float strength, float health) : base(position)
    {
        Sprite = sprite;
        // Hold the depth for the monster
        CurrentDepth = currentDepth;
        // Get a reference to the world
        _world = world;
        // Set the health and strength
        _strength = strength;
        _health = health;
        // Get reference to the monster manager
        _monsterManager = monsterManager;
        // Add to the monster manager
        _monsterManager.AddMonster(this);
        // Load the sound
        _deathSound = Raylib.LoadSound("Assets/sound/21_orc_damage_1.wav");
    }
    // On destruction, remove the death sound
    ~Monster()
    {
        Raylib.UnloadSound(_deathSound);
    }

    public override void Render(Rectangle cameraBounds)
    {
        // Draw the sprite at our monster position
        Sprite.Draw(Position);
    }
    public override void Update(float deltaTime)
    {
        return;
    }

    public bool TakeDamage(float damage)
    {
        // First remove the damage from the health
        _health -= damage;
        // If we fall below zero health then remove from the game
        if(_health <=0 && _monsterManager != null)
        {
            // If the monster is killed then remove
            _monsterManager.RemoveMonster(this);
            Raylib.PlaySound(_deathSound);
            return true;
        }
        // Return false when the monster is not killed
        return false;
    }
}

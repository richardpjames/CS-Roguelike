using Raylib_cs;
using System.Numerics;

public class MonsterManager
{
    private List<Monster> _monsters;
    public int CurrentDepth;

    public MonsterManager()
    {
        // Initialise our list of monsters
        _monsters = new List<Monster>();
        CurrentDepth = 0;
    }

    // Add a new monster to the list
    public void AddMonster(Monster monster)
    {
        _monsters.Add(monster);
    }

    public Monster? GetMonster(Vector2 position, int depth)
    {
        return _monsters.FirstOrDefault<Monster>((monster) => { return monster.Position.X == position.X && monster.Position.Y == position.Y && monster.CurrentDepth == depth; });
    }

    public void RemoveMonster(Monster monster)
    {
        // If we have a record of the monster then remove it
        if (_monsters.Contains(monster))
        {
            _monsters.Remove(monster);
        }
    }

    // Render each of the monsters
    public void Render(Rectangle cameraBounds)
    {
        foreach (Monster monster in _monsters)
        {
            if (monster.CurrentDepth == CurrentDepth)
            {
                monster.Render(cameraBounds);
            }
        }
    }

    // Update each of the monsters
    public void Update(float deltaTime)
    {
        foreach (Monster monster in _monsters)
        {
            monster.Update(deltaTime);
        }
    }
}

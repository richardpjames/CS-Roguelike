using Raylib_cs;
using System.Numerics;

public abstract class GameObject
{
    public Vector2 Position;
    public GameObject(Vector2 position)
    {
        this.Position = position;
    }

    public abstract void Update(float deltaTime);
    public abstract void Render(Rectangle cameraBounds);

}


using Raylib_cs;
using System.Numerics;

public class Game
{
    private Player _player;
    private World _world;
    private Camera2D _camera;
    // Determine whether the game is running
    public bool Running
    {
        get
        {
            // While the window is not signalling it should close then continue the game
            if (!Raylib.WindowShouldClose()) return true;
            return false;
        }
    }
    public Game()
    {
        // Create a window which is the resolution of the monitor
        int monitor = Raylib.GetCurrentMonitor();
        Raylib.InitWindow(Raylib.GetMonitorWidth(monitor), Raylib.GetMonitorHeight(monitor), "RogueLike Adventure");
        // Set to full screen
        Raylib.ToggleBorderlessWindowed();
        // Create our player - starting with the sprite
        Sprite playerSprite = new Sprite("Assets/rogues.png", new Rectangle(0, 32, 32, 32));
        // Then create the player itself at 0,0
        _player = new Player(new Vector2(192, 224), playerSprite);
        // Create the world from the LDTK file
        _world = new World("Assets/World.ldtk");
        // Initialise the camera - sets the offset to center the screen, pointing at zero,zero, with no rotation but zoomed
        _camera = new Camera2D(new Vector2((Raylib.GetScreenWidth() / 2), (Raylib.GetScreenHeight() / 2)), new Vector2(0, 0), 0, 3);
    }
    // On destruction we clean up the window etc.
    ~Game()
    {
        // Close the window and OpenGL context
        Raylib.CloseWindow();
    }

    public void Update()
    {

        // Get the frame time from Raylib (create our deltaTime for updates)
        float deltaTime = Raylib.GetFrameTime();
        // Update the world
        _world.Update(deltaTime);
        // Update the player
        _player.Update(deltaTime);
        // Update the camera to follow the player
        _camera.Target = _player.Position;
    }

    public void Render()
    {
        // For now clear a white window and draw hello world
        Raylib.BeginDrawing();
        Raylib.BeginMode2D(_camera);
        // Clear the window with a gray background
        Raylib.ClearBackground(Color.Black);
        // Determine the camera bounds based on the target, the offset (adjusted by zoom) and the width and height (also adjusted by zoom)
        Rectangle cameraBounds = new Rectangle((_camera.Target.X - (_camera.Offset.X / _camera.Zoom)),
            (_camera.Target.Y - (_camera.Offset.Y / _camera.Zoom)),
            (Raylib.GetScreenWidth() / _camera.Zoom),
            (Raylib.GetScreenHeight() / _camera.Zoom));
        // Render the world
        _world.Render(cameraBounds);
        // Render the player object
        _player.Render(cameraBounds);
        // End the camera and drawing modes
        Raylib.EndMode2D();
        Raylib.EndDrawing();
    }
}

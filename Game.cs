﻿using Raylib_cs;
using System.Numerics;

public class Game
{
    // Constants for across the project
    public const int PIXELS_PER_UNIT = 32;
    public const int SCREEN_WIDTH = 1920;
    public const int SCREEN_HEIGHT = 1080;

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
        Raylib.InitWindow(SCREEN_WIDTH, SCREEN_HEIGHT, "RogueLike Adventure");
        // Set the window icon
        Raylib.SetWindowIcon(Raylib.LoadImage("Assets/windowicon.png"));
        //Raylib.InitWindow(Raylib.GetMonitorWidth(monitor), Raylib.GetMonitorHeight(monitor), "RogueLike Adventure");
        // Set to full screen
        //Raylib.ToggleBorderlessWindowed();
        // Create the world from the LDTK file
        _world = new World("Assets/World.ldtk");
        // Create our player - starting with the sprite
        Texture2D rogues = Raylib.LoadTexture("Assets/rogues.png");
        Sprite playerSprite = new Sprite(rogues, new Rectangle(0, 1, 1, 1).GridToWorld());
        // Then create the player itself at 0,0
        _player = new Player(new Vector2(6, 7), playerSprite, _world);
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
        _camera.Target = _player.Position.GridToWorld();
        // Handle input for zooming the camera
        CameraZoom();
    }

    private void CameraZoom()
    {
        // Check if camera zoom needs updating
        if (Raylib.IsKeyReleased(KeyboardKey.PageUp))
        {
            // Page up zooms out
            if (_camera.Zoom < 5)
            {
                _camera.Zoom++;
            }
        }
        else if (Raylib.IsKeyReleased(KeyboardKey.PageDown))
        {
            // Page down zooms in
            if (_camera.Zoom > 1)
            {
                _camera.Zoom--;
            }
        }
    }

    public void Render()
    {
        // For now clear a white window and draw hello world
        Raylib.BeginDrawing();
        Raylib.BeginMode2D(_camera);
        // Clear the window with a gray background
        Raylib.ClearBackground(Color.Red);
        // Determine the camera bounds based on the target, the offset (adjusted by zoom) and the width and height (also adjusted by zoom)
        int screenWidth = Raylib.GetScreenWidth();
        int screenHeight = Raylib.GetScreenHeight();
        Rectangle cameraBounds = new Rectangle((_camera.Target.X - (_camera.Offset.X / _camera.Zoom)),
            (_camera.Target.Y - (_camera.Offset.Y / _camera.Zoom)),
            (screenWidth / _camera.Zoom),
            (screenHeight / _camera.Zoom));
        Raylib.DrawRectangleRec(cameraBounds, Color.Black);
        // Render the world
        _world.Render(cameraBounds);
        // Render the player object
        _player.Render(cameraBounds);
        // End the camera and drawing modes
        Raylib.EndMode2D();
        Raylib.EndDrawing();
    }
}

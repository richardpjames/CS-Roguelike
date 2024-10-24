using Raylib_cs;
using System.Numerics;

public static class Vector2Extensions
{
    // Adds a method to Vector2 objects to convert from game space to pixel space (multiply by pixels per unit)
    public static Vector2 WorldToPixel(this Vector2 vector)
    {
        return vector * Game.PIXELS_PER_UNIT;
    }

    // Turns screen coordinates to world coordinates accounting for intermediate translation of pixels per inch
    public static Vector2 ScreenToWorld(this Vector2 vector, Camera2D camera)
    {
        Vector2 newVector = Raylib.GetScreenToWorld2D(vector, camera) / Game.PIXELS_PER_UNIT;
        newVector.X = MathF.Floor(newVector.X);
        newVector.Y = MathF.Floor(newVector.Y);
        return newVector;
    }

}


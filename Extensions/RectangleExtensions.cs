using Raylib_cs;

public static class RectangleExtensions
{
    public static Rectangle GridToWorld(this Rectangle rectangle)
    {
        Rectangle newRectangle = rectangle;
        newRectangle.X *= Game.PIXELS_PER_UNIT;
        newRectangle.Y *= Game.PIXELS_PER_UNIT;
        newRectangle.Width *= Game.PIXELS_PER_UNIT;
        newRectangle.Height *= Game.PIXELS_PER_UNIT;
        return newRectangle;
    }

    public static Rectangle WorldToGrid(this Rectangle rectangle)
    {
        Rectangle newRectangle = rectangle;
        newRectangle.X = MathF.Floor(newRectangle.X / Game.PIXELS_PER_UNIT);
        newRectangle.Y = MathF.Floor(newRectangle.Y / Game.PIXELS_PER_UNIT);
        newRectangle.Width = MathF.Floor(newRectangle.Width / Game.PIXELS_PER_UNIT);
        newRectangle.Height = MathF.Floor(newRectangle.Height / Game.PIXELS_PER_UNIT);
        return newRectangle;
    }
}


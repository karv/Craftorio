namespace Craftorio;

/// <summary>
/// An entity with this component is located in the world.
/// </summary>
public record struct Location
{
    public void Offset(in Vector2 offset)
    {
        Bounds.Offset(offset);
    }

    public Location(in RectangleF bounds)
    {
        Bounds = bounds;
    }

    public static explicit operator Location(in RectangleF bounds)
    {
        return new Location(bounds);
    }

    public RectangleF Bounds;

    /// <summary>
    /// The position of the entity.
    /// </summary>
    public readonly Vector2 AsVector => Bounds.TopLeft;

    /// <summary>
    /// Gets the X coordinate.
    /// </summary>

    public readonly float X => Bounds.Left;

    /// <summary>
    /// Gets the Y coordinate.
    /// </summary>
    public readonly float Y => Bounds.Right;
    public readonly float Left => Bounds.Left;
    public readonly float Right => Bounds.Right;
    public readonly float Top => Bounds.Top;
    public readonly float Bottom => Bounds.Bottom;
    public readonly float Width => Bounds.Width;
    public readonly float Height => Bounds.Height;
}
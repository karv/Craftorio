namespace Craftorio;

/// <summary>
/// An entity with this component is located in the world.
/// </summary>
public record struct Location
{
    /// <summary>
    /// Offsets the position of the entity.
    /// </summary>
    /// <param name="offset"></param>
    public void Offset(in Vector2 offset)
    {
        Bounds.Offset(offset);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Location"/> struct.
    /// </summary>
    public Location(in RectangleF bounds)
    {
        Bounds = bounds;
    }

    /// <summary>
    /// Converts a <see cref="RectangleF"/> into a <see cref="Location"/>.
    /// </summary>
    public static explicit operator Location(in RectangleF bounds)
    {
        return new Location(bounds);
    }

    /// <summary>
    /// The bounds of the entity.
    /// </summary>
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

    /// <summary>
    /// Gets the left coordinate.
    /// </summary>
    public readonly float Left => Bounds.Left;

    /// <summary>
    /// Gets the right coordinate.
    /// </summary>
    public readonly float Right => Bounds.Right;

    /// <summary>
    /// Gets the top coordinate.
    /// </summary>
    public readonly float Top => Bounds.Top;

    /// <summary>
    /// Gets the bottom coordinate.
    /// </summary>
    public readonly float Bottom => Bounds.Bottom;

    /// <summary>
    /// Gets the width.
    /// </summary>
    public readonly float Width => Bounds.Width;

    /// <summary>
    /// Gets the height.
    /// </summary>
    public readonly float Height => Bounds.Height;
}
namespace Craftorio;

/// <summary>
/// Represents a set of values required to convert a point to a rectangle.
/// </summary>
public struct RectangleExpand
{

    /// <summary>
    /// Gets the bottom value.
    /// </summary>
    public float Bottom;
    /// <summary>
    /// Gets the left value.
    /// </summary>
    public float Left;

    /// <summary>
    /// Gets the right value.
    /// </summary>
    public float Right;

    /// <summary>
    /// Gets the top value.
    /// </summary>
    public float Top;

    /// <summary>
    /// Gets the height of any rectangle created with <see cref="Expand"/>.
    /// </summary>
    public readonly float Height => Top + Bottom;

    /// <summary>
    /// Gets the width of any rectangle created with <see cref="Expand"/>.
    /// </summary>
    public readonly float Width => Left + Right;

    /// <summary>
    /// Gets the rectangle that is expanded by the specified values.
    /// </summary>
    /// <param name="center">The pivot point of stretching.</param>
    public readonly RectangleF Expand(in Vector2 center)
    {
        return new RectangleF(
            center.X - Left,
            center.Y - Top,
            Width,
            Height);
    }
}
namespace Craftorio;

/// <summary>
/// An entity with this component is located in the world.
/// </summary>
public record struct Location
{
    /// <summary>
    /// The position of the entity.
    /// </summary>
    public Vector2 AsVector;

    /// <summary>
    /// Gets the X coordinate.
    /// </summary>

    public readonly float X => AsVector.X;

    /// <summary>
    /// Gets the Y coordinate.
    /// </summary>
    public readonly float Y => AsVector.Y;
}
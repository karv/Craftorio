namespace Craftorio;

/// <summary>
/// An entity with this component is located in the world.
/// </summary>
public record struct Location
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Location"/> struct.
    /// </summary>
    public Location(in Vector2 vector)
    {
        AsVector = vector;
    }

    /// <summary>
    /// Converts a vector to a location.
    /// </summary>
    public static explicit operator Location(in Vector2 vector) => new(vector);

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
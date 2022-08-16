namespace Craftorio;

/// <summary>
/// Entities this this component will be forced to be at the mouse location.
/// </summary>
public record struct ForceMouseLocation
{
    /// <summary>
    /// Offset (in world coordinates) from the mouse location.
    /// </summary>
    public Vector2 Offset;
}
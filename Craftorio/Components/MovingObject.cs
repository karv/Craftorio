namespace Craftorio;

/// <summary>
/// An entity with this component is moving toward a target (entity).
/// </summary>
public record struct MovingObject
{
    /// <summary>
    /// The target entity.
    /// </summary>
    public Entity TargetEntity;

    /// <summary>
    /// The speed of the entity, in units per millisecond.
    /// </summary>
    public float MoveSpeed;
}
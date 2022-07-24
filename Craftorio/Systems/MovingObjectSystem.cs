namespace Craftorio;
using DefaultEcs.System;

/// <summary>
/// Controls the movement of entities whose target is another entity.
/// </summary>
public class MovingObjectSystem : AEntitySetSystem<int>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MovingObjectSystem"/> class.
    /// </summary>
    public MovingObjectSystem(World world) :
        base(world.GetEntities()
            .With<Location>()
            .With<MovingObject>()
            .AsSet())
    {
    }

    /// <summary>
    /// Changes the position of the specified entity, moving toward its defined target.
    /// </summary>
    protected override void Update(int state, in Entity entity)
    {
        // Move the entity toward the target location.
        ref var location = ref entity.Get<Location>();
        ref var movingObject = ref entity.Get<MovingObject>();
        var deltaPosition = (movingObject.TargetEntity.Get<Location>().AsVector - location.AsVector);
        deltaPosition.Normalize();
        deltaPosition *= movingObject.MoveSpeed;
        location.Offset(deltaPosition);
    }
}
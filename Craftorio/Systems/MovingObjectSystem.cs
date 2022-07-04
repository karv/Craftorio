namespace Craftorio;
using DefaultEcs.System;

public class MovingObjectSystem : AEntitySetSystem<int>
{
    public MovingObjectSystem(World world) :
        base(world.GetEntities()
            .With<Location>()
            .With<MovingObject>()
            .AsSet())
    {
    }

    protected override void Update(int state, in Entity entity)
    {
        // Move the entity toward the target location.
        ref var location = ref entity.Get<Location>();
        ref var movingObject = ref entity.Get<MovingObject>();
        var deltaPosition = (movingObject.TargetEntity.Get<Location>().AsVector - location.AsVector);
        deltaPosition.Normalize();
        deltaPosition *= movingObject.MoveSpeed;
        location.AsVector += deltaPosition;
    }
}
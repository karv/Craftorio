namespace Craftorio.Construction;

using DefaultEcs.System;
using Production;

/// <summary>
/// Detects if a constructor arrives at its target construction site.
/// </summary>
public class ConstructorArriveSystem : AEntitySetSystem<int>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConstructorArriveSystem"/> class.
    /// </summary>
    public ConstructorArriveSystem(World world) :
        base(world.GetEntities()
            .With<Logistic.ConstructorData>()
            .With<Location>()
            .With<MovingObject>()
            .AsSet(),
            useBuffer: true) // As the constructors are going to vanish, we need to use a buffer.
    { }

    /// <summary>
    /// Determines if the specified constructor has arrived at its target. If so do the following:
    /// 1. If the destination is a construction site, add the constructor to the list of active constructors.
    ///    and destroy the constructor from the world
    /// 2. If the destination is a base, remove the constructor from the list of active constructors.
    /// </summary>
    protected override void Update(int state, in Entity entity)
    {
        ref var constructor = ref entity.Get<Logistic.ConstructorData>();
        ref var location = ref entity.Get<Location>();
        ref var movingObject = ref entity.Get<MovingObject>();
        ref var target = ref movingObject.TargetEntity.Get<Location>();
        var tickDisplacement = (int)movingObject.MoveSpeed * state;
        var distance = (location.AsVector - target.AsVector).Length();
        // If the constructor is at the location:
        if (distance <= tickDisplacement)
        {
            // determine whether the location is a construction site or a base node.
            if (movingObject.TargetEntity.Has<Constructing>())
            {
                // If the location is a construction site, then the constructor is going to build it.
                // Destroy the constructor, and add the construction speed to the construction site.
                ref var timer = ref movingObject.TargetEntity.Get<TimeConsumption>();
                timer.Speed += constructor.ConstructionSpeed;
                // the target entity is a construction site; add the constructor to the list of constructors.
                movingObject.TargetEntity.Get<Constructing>().ActiveConstructors.Add(entity);
                entity.Disable();  // Or Dispose?
            }
            else
            {
                // If the location is a base node, then the constructor is going to rest. Dispose it.
                movingObject.TargetEntity.Get<Logistic.NodeBase>().ConstructorCount++;
                entity.Dispose();
            }
        }
    }
}

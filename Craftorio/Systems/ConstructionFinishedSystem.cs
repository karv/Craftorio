namespace Craftorio.Construction;
using DefaultEcs.System;
using Production;

/// <summary>
/// Constructs the building when the construction timer is completed.
/// </summary>
public class ConstructionFinishedSystem : AEntitySetSystem<int>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConstructionFinishedSystem"/> class.
    /// </summary>
    public ConstructionFinishedSystem(World world) :
        base(world.GetEntities()
            .With<Constructing>()
            .With<TimeConsumption>()  // Use WithChanged instead?
            .AsSet(),
            useBuffer: true)
    { }

    /// <summary>
    /// Verifies if the entity is ready to be constructed.
    /// If constructed this method will:
    /// 1. Remove the constructing entity.
    /// 2. Add the constructed entity following its prototype, at the same location.
    /// 3. Free all constructors and send them to the base.
    /// </summary>
    protected override void Update(int state, in Entity entity)
    {
        ref var constructing = ref entity.Get<Constructing>();
        ref var timer = ref entity.Get<TimeConsumption>();
        if (timer.IsCompleted)
        {
            // Create the building.
            var building = World.CreateEntity();
            building.Set(entity.Get<Location>());
            constructing.Prototype.AddComponentsTo(building);

            // Restore the constructors, and send them back to the base.
            foreach (var constructor in constructing.ActiveConstructors)
            {
                var network = constructor.Get<Logistic.ConstructorData>().Network;
                constructor.Get<Logistic.ConstructorData>().State = Logistic.ConstructorData.ConstructorState.ReturnToBase;
                var location = constructor.Get<Location>().AsVector;
                if (network.TryGetClosestBase(location, out var baseEntity))
                {
                    constructor.Get<MovingObject>().TargetEntity = baseEntity;
                    constructor.NotifyChanged<MovingObject>();
                    constructor.Enable();
                }
                else
                    // As there is no base, destroy the constructor.
                    constructor.Dispose();
            }

            entity.Dispose();
        }
    }
}
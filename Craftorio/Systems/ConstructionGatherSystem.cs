namespace Craftorio.Construction;
using System.Runtime.CompilerServices;
using DefaultEcs.System;

/// <summary>
/// Iterates through all the entities considered to be under construction (with Constructing) and updates their progress.
/// Its sole purpose is to update the progress of the construction in step 1; and if required, to change to step 2.
/// </summary>
public class ConstructionGatherSystem : AEntitySetSystem<int>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConstructionGatherSystem"/> class.
    /// </summary>
    public ConstructionGatherSystem(World world) :
        base(world.GetEntities()
            .With<Constructing>()
            .With<Location>()
            // The box ensures that the entity is in the gather state.
            .With<IBox>()
            // We do not add the component Requesting here, because resources may be put into the box by other means.
            .AsSet(),
            useBuffer: true) // As the entities morph enough to change the set, we need to use a buffer.
    {
    }

    /// <summary>
    /// Determine if the content of the IBox component contains the required resources as in the constructing component.
    /// </summary>
    protected override void Update(int state, in Entity entity)
    {
        ref var constructing = ref entity.Get<Constructing>();
        ref var box = ref entity.Get<IBox>();
        if (constructing.RequiredResources is not null && BoxContainsAllDictionary(box, constructing.RequiredResources))
        {
            // Change the state of the entity to constructing:
            // - remove the components: IBox, RequestData
            // - add the component: TimeConsumption with the required time and initialize with speed 0.

            entity.Remove<IBox>();
            entity.Remove<Logistic.RequestData>();
            entity.Set(new Production.TimeConsumption
            {
                Cost = constructing.RequiredTime,
                Speed = 0,
                ProductionState = Production.ProductionState.Working // This is required by the TimeConsumptionSystem.
            });
        }
    }

    /// <summary>
    /// Determines if a box contains all the items in a dictionary.
    /// </summary>
    [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
    private static bool BoxContainsAllDictionary(IBox box, Dictionary<int, int> dictionary)
    {
        foreach (var (key, value) in dictionary)
            if (box[key] < value)
                return false;

        return true;
    }
}
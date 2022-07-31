namespace Craftorio.Production;

/// <summary>
/// Controls the behavior of miners, by creating target items when their cycle is completed.
/// </summary>
public class MiningSystem : DefaultEcs.System.AEntitySetSystem<int>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MiningSystem"/> class.
    /// </summary>
    public MiningSystem(World world) :
    base(world.GetEntities()
        .With<IBox>()
        .With<TimeConsumption>()
        .With<ItemTarget>()
        .AsSet())
    {
    }

    /// <summary>
    /// If the specified miner entity completed a mining cycle, create a target item into its box.
    /// </summary>
    protected override void Update(int state, in Entity entity)
    {
        // Update the state of the entity's time consumption.
        ref var timeConsumption = ref entity.Get<TimeConsumption>();
        if (timeConsumption.Progress >= timeConsumption.Cost)
        {
            // The entity has finished its time consumption.
            // If there is space in the output box, reset the TimeConsumption component, and put a new item of the mining type onto the output box.
            Box outputBox = (Box)entity.Get<IBox>();
            if (outputBox.TryStore(entity.Get<ItemTarget>().ItemId, 1))
            {
                timeConsumption.Reset();
                World.Publish(new MiningCompleted { Miner = entity });
            }
        }
    }
}
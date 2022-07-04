namespace Craftorio.Production;

public class MiningSystem : DefaultEcs.System.AEntitySetSystem<int>
{
    public MiningSystem(World world) :
    base(world.GetEntities()
        .With<IOutputBox>()
        .With<TimeConsumption>()
        .With<ItemTarget>()
        .AsSet())
    {
    }

    protected override void Update(int state, in Entity entity)
    {
        // Update the state of the entity's time consumption.
        ref var timeConsumption = ref entity.Get<TimeConsumption>();
        if (timeConsumption.Progress >= timeConsumption.Cost)
        {
            // The entity has finished its time consumption.
            // If there is space in the output box, reset the TimeConsumption component, and put a new item of the mining type onto the output box.
            Box outputBox = (Box)entity.Get<IOutputBox>();
            if (outputBox.TryStore(entity.Get<ItemTarget>().ItemId, 1))
            {
                timeConsumption.Reset();
                World.Publish(new MiningCompleted { Miner = entity });
            }
        }
    }
}
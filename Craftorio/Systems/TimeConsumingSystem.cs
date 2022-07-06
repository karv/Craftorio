namespace Craftorio.Production;
using DefaultEcs.System;

public class TimeConsumingSystem : AComponentSystem<int, TimeConsumption>
{
    public TimeConsumingSystem(World world) : base(world)
    {
    }

    protected override void Update(int state, ref TimeConsumption component)
    {
        if (component.Progress >= component.Cost | component.ProductionState != ProductionState.Working) return;

        // Update the state of the entity's time consumption.
        component.Progress += (int)(state * component.Speed);
        if (component.Progress >= component.Cost)
            component.Progress = component.Cost;
    }
}

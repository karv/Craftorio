namespace Craftorio.Production;
using DefaultEcs.System;

/// <summary>
/// Updates the state of entities that have a progress bar stored in a <see cref="TimeConsumption"/> component.
/// </summary>
public class TimeConsumingSystem : AComponentSystem<int, TimeConsumption>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TimeConsumingSystem"/> class.
    /// </summary>
    public TimeConsumingSystem(World world) : base(world)
    {
    }

    /// <summary>
    /// Fill a bit of the progress bar of the specified component, depending on the elapsed time.
    /// </summary>
    protected override void Update(int state, ref TimeConsumption component)
    {
        if (component.IsCompleted | component.ProductionState != ProductionState.Working) return;

        // Update the state of the entity's time consumption.
        component.Progress += (int)(state * component.Speed);
        if (component.Progress >= component.Cost)
            component.Progress = component.Cost;
    }
}
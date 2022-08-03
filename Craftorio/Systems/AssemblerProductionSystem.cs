namespace Craftorio.Production;

/// <summary>
/// Controls the production of entities based of <see cref="Craftorio.Production.Recipe"/>.
/// </summary>
public sealed class AssemblerProductionSystem : DefaultEcs.System.AEntitySetSystem<int>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblerProductionSystem"/> class.
    /// </summary>
    public AssemblerProductionSystem(World world) :
    base(world.GetEntities()
        .With<IBox>()            // To take the resources from/to
        .With<TimeConsumption>() // To consume the time
        .With<RecipeComponent>() // The recipe to use
        .AsSet())
    { }

    /// <summary>
    /// Updates the state of the specified entity.
    /// </summary>
    protected override void Update(int state, in Entity entity)
    {
        ref var timeConsumption = ref entity.Get<TimeConsumption>();
        ref readonly var recipe = ref entity.Get<RecipeComponent>();

        if (timeConsumption.IsCompleted || timeConsumption.ProductionState == ProductionState.OutputFull)
        {
            // The entity has finished its time consumption.
            // If there is space in the output box, reset the TimeConsumption component, and put a new item of the mining type onto the output box.
            var outputBox = entity.Get<IBox>();

            // If there is space in the output box, reset the TimeConsumption component
            // and put a new item of the output of the recipe type onto the output box.
            if (outputBox.TryStore(recipe.Outputs))
            {
                timeConsumption.Reset();
                World.Publish(new ProductionCompleted(entity));

                // Take the resources from the input box.
                var inputBox = entity.Get<IBox>();
                if (!inputBox.TryRemoveItems(recipe.Inputs))
                {
                    timeConsumption.ProductionState = ProductionState.WaitingForResources;
                    World.Publish(new ProductionStateChanged(entity));
                }
            }
            else
            {
                // Do not change the state if the current state is OutputFull.
                if (timeConsumption.ProductionState != ProductionState.OutputFull)
                {
                    timeConsumption.ProductionState = ProductionState.OutputFull;
                    World.Publish(new ProductionStateChanged(entity));
                }
            }
        }
        else
        if (timeConsumption.ProductionState == ProductionState.WaitingForResources)
        {
            // Waiting for the entity to get the required resources
            // If the entity has the required resources, start working
            var inputBox = entity.Get<IBox>();
            if (inputBox.TryRemoveItems(recipe.Inputs))
            {
                timeConsumption.ProductionState = ProductionState.Working;
                World.Publish(new ProductionStateChanged(entity));
            }
        }
    }
}
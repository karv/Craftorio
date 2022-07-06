namespace Craftorio.Production;

public sealed class AssemblerProductionSystem : DefaultEcs.System.AEntitySetSystem<int>
{
    public AssemblerProductionSystem(World world) :
    base(world.GetEntities()
        .With<IStoreBox>()  // To take the resources from
        .With<ITakeableBox>() // To put the resources into
        .With<TimeConsumption>() // To consume the time
        .With<RecipeComponent>() // The recipe to use
        .AsSet())
    { }

    protected override void Update(int state, in Entity entity)
    {
        ref var timeConsumption = ref entity.Get<TimeConsumption>();
        ref readonly var recipe = ref entity.Get<RecipeComponent>();

        if (timeConsumption.IsCompleted || timeConsumption.ProductionState == ProductionState.OutputFull)
        {
            // The entity has finished its time consumption.
            // If there is space in the output box, reset the TimeConsumption component, and put a new item of the mining type onto the output box.
            var outputBox = entity.Get<IStoreBox>();

            // If there is space in the output box, reset the TimeConsumption component
            // and put a new item of the output of the recipe type onto the output box.
            if (outputBox.TryStore(recipe.Outputs))
                timeConsumption.Reset();
            else timeConsumption.ProductionState = ProductionState.OutputFull;
        }
        else
        if (timeConsumption.ProductionState == ProductionState.WaitingForResources)
        {
            // Waiting for the entity to get the required resources
            // If the entity has the required resources, start working
            var inputBox = entity.Get<ITakeableBox>();
            if (inputBox.TryRemoveItems(recipe.Inputs))
                timeConsumption.ProductionState = ProductionState.Working;
        }
    }
}
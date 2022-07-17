namespace Craftorio;
using Production;
using Logistic;

/// <summary>
/// Module that exposes method for commonly used entity types.
/// </summary>
public static class EntityFactory
{
    /// <summary>
    /// Creates an assembler-type entity.
    /// </summary>
    /// <param name="world">ECS World.</param>
    /// <param name="location">Location of the entity.</param>
    /// <param name="recipe">Recipe of the assembler.</param>
    /// <param name="includeLogisticSupport">is true, will add logistic requester-providers
    /// depending on the recipe</param>
    /// <param name="speed">the speed multiplier of the assembler</param>
    /// <returns>The created entity.</returns>
    public static Entity CreateAssembler(World world, RectangleF location, Recipe recipe,
    bool includeLogisticSupport = false,
    float speed = 1f)
    {
        Box iBox = new();
        Box oBox = new();
        var assembler = world.CreateEntity();
        assembler.Set<IStoreBox>(iBox);
        assembler.Set<ITakeableBox>(oBox);
        assembler.Set<TimeConsumption>(new TimeConsumption
        {
            Cost = recipe.BaseTime,
            Speed = speed,
            ProductionState = ProductionState.WaitingForResources
        });
        assembler.Set(new Location(location));
        assembler.Set(recipe.ToComponent());
        if (includeLogisticSupport)
        {
            var provData = new ProvideData();
            var reqData = new RequestData();
            // Add the request data to the assembler
            foreach (var input in recipe.Inputs)
                reqData.AddRequest(input.ItemId, input.Count);
            assembler.Set<ProvideData>(provData);
            assembler.Set<RequestData>(reqData);
        }
        assembler.Set(new Drawing.Sprite { Color = Color.Red });
        return assembler;
    }

    /// <summary>
    /// Creates a base node entity.
    /// </summary>
    /// <param name="world">ECS world.</param>
    /// <param name="location">Location of the entity.</param>
    /// <returns>The created entity.</returns>
    public static Entity CreateBase(World world, RectangleF location)
    {
        var baseEntity = world.CreateEntity();
        baseEntity.Set(new Location(location));
        baseEntity.Set<Logistic.NodeBase>();
        baseEntity.Set(new Drawing.Sprite { Color = Color.White });

        return baseEntity;
    }

    /// <summary>
    /// Creates a miner entity.
    /// </summary>
    /// <param name="world">ECS world.</param>
    /// <param name="location">Location of the entity.</param>
    /// <param name="Cost">Cost in milliseconds of every mined items.</param>
    /// <param name="Speed">Speed multiplier.</param>
    /// <param name="ItemId">Item that is mined.</param>
    /// <returns></returns>
    public static Entity CreateMiner(World world, RectangleF location,
    int Cost = 1000,
    float Speed = 1f,
    int ItemId = 1)
    {
        var box = new Box();
        var miner = world.CreateEntity();
        var pData = new Logistic.ProvideData();
        miner.Set<ITakeableBox>(box);
        miner.Set<Production.TimeConsumption>(new Production.TimeConsumption
        {
            Cost = Cost,
            Speed = Speed,
            ProductionState = ProductionState.Working
        });
        miner.Set<Production.ItemTarget>(new Production.ItemTarget { ItemId = ItemId });
        miner.Set<Logistic.ProvideData>(pData);
        miner.Set(new Location(location));
        miner.Set(new Drawing.Sprite { Color = Color.Green });
        return miner;
    }

    /// <summary>
    /// Creates a storage entity with access to the logistic network.
    /// </summary>
    /// <param name="world">ECS world.</param>
    /// <param name="location">Location of the entity.</param>
    /// <param name="box">If not null, the storage box.</param>
    /// <param name="requests">If not null, the requested items.</param>
    public static Entity CreateStorageBox(World world, RectangleF location,
    Box? box = null,
    int[]? requests = null
    )
    {
        box ??= new Box();
        requests ??= new int[0];
        var storageBox = world.CreateEntity();
        storageBox.Set<ITakeableBox>(box);
        storageBox.Set<IStoreBox>(box);
        var req = new Logistic.RequestData();
        foreach (var item in requests)
            req.AddRequest(item, int.MaxValue);
        storageBox.Set<Logistic.RequestData>(req);
        storageBox.Set<Location>(new Location(location));
        storageBox.Set(new Drawing.Sprite { Color = Color.Yellow });
        storageBox.Set(new Drawing.UI.MouseOverDisplayText
        {
            GetText = (Entity entity) => entity.Get<IStoreBox>().DisplayContent()
        });

        return storageBox;
    }
}
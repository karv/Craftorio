namespace Craftorio;
using Production;
using Logistic;

public static class EntityFactory
{

    public static Entity CreateAssembler(World world, Vector2 location, Recipe recipe,
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
        assembler.Set(new Location { AsVector = location });
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
        return assembler;
    }

    public static Entity CreateBase(World world, Vector2 location)
    {
        var baseEntity = world.CreateEntity();
        baseEntity.Set(new Location { AsVector = location });
        baseEntity.Set<Logistic.NodeBase>();

        return baseEntity;
    }
    public static Entity CreateMiner(World world, Vector2 location,
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
        pData.EnsureDictionaryKeys(new[] { ItemId });
        miner.Set(new Location { AsVector = location });
        return miner;
    }

    public static Entity CreateStorageBox(World world, Vector2 location,
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
        req.EnsureDictionaryKeys();
        storageBox.Set<Location>(new Location { AsVector = location });
        return storageBox;
    }
}
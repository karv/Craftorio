namespace Craftorio;

public static class EntityFactory
{

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
        miner.Set<IOutputBox>(box);
        miner.Set<Production.TimeConsumption>(new Production.TimeConsumption { Cost = Cost, Speed = Speed });
        miner.Set<Production.ItemTarget>(new Production.ItemTarget { ItemId = ItemId });
        miner.Set<Logistic.ProvideData>(pData);
        pData.EnsureDictionaryKeys(new[] { ItemId });
        miner.Set(new Location { AsVector = location });
        return miner;
    }

    public static Entity CreateStorageBox(World world,
    Vector2 location,
    Box? box = null,
    int[]? requests = null
    )
    {
        box ??= new Box();
        requests ??= new int[0];
        var storageBox = world.CreateEntity();
        storageBox.Set<IOutputBox>(box);
        storageBox.Set<IInputBox>(box);
        var reqDictionary = new Dictionary<int, int>();
        var req = new Logistic.RequestData() { RequestDictionary = reqDictionary };
        foreach (var item in requests)
        {
            reqDictionary.Add(item, int.MaxValue);
        }
        storageBox.Set<Logistic.RequestData>(req);
        req.EnsureDictionaryKeys();
        storageBox.Set<Location>(new Location { AsVector = location });
        return storageBox;
    }
}
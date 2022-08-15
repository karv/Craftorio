namespace Craftorio;
using Logistic;
using Production;

/// <summary>
/// Module that exposes method for commonly used entity types.
/// </summary>
public static class EntityFactory
{
    /// <summary>
    /// The default value for the RelativeDrawingArea value in the Sprite component.
    /// </summary>
    public static readonly RectangleExpand DefaultSpriteSize = new Craftorio.RectangleExpand
    {
        Left = 15,
        Right = 16,
        Top = 15,
        Bottom = 16
    };

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
    public static Entity CreateAssembler(World world, Vector2 location, Recipe recipe,
        bool includeLogisticSupport = false,
        float speed = 1f)
    {
        Box box = new();
        var assembler = world.CreateEntity();
        assembler.Set<IBox>(box);
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
        assembler.Set(new Drawing.Sprite { Color = Color.Red, RelativeDrawingArea = DefaultSpriteSize });
        assembler.Set(new Drawing.UI.MouseOverDisplayText
        {
            GetText = GetTooltip,
            RelativeSensibleArea = DefaultSpriteSize
        });

        static string GetTooltip(Entity entity)
        {
            var timer = entity.Get<TimeConsumption>();
            return $"{timer.ProductionState}#{timer.Progress}/{timer.Cost}:{entity.Get<RecipeComponent>().Outputs[0].Count}";
        }

        return assembler;
    }

    /// <summary>
    /// Creates a base node entity.
    /// </summary>
    /// <param name="world">ECS world.</param>
    /// <param name="location">Location of the entity.</param>
    /// <param name="network">The network where this base belong.</param>
    /// <param name="carrierCount"> How many carriers are in this base.</param>
    /// <param name="constructorCount">How many constructors are in this base.</param>
    /// <param name="carrierDeploySpeed">How fast the carriers are deployed, in carriers per second.</param>
    /// <returns>The created entity.</returns>
    public static Entity CreateBase(
        World world, Vector2 location, LogisticNetwork network,
        int carrierCount = 10,
        int constructorCount = 10,
        float carrierDeploySpeed = 1f)
    {
        var baseEntity = world.CreateEntity();
        baseEntity.Set(new Location(location));
        baseEntity.Set<Logistic.NodeBase>(new NodeBase
        {
            CarrierCount = carrierCount,
            ConstructorCount = constructorCount,
            OrdersQueue = new CE.Collections.Queue<LogisticOrder>(),
            Network = network
        });
        baseEntity.Set(new Drawing.Sprite { Color = Color.White, RelativeDrawingArea = DefaultSpriteSize });
        baseEntity.Set(new TimeConsumption
        {
            Cost = 1000,
            Speed = carrierDeploySpeed,
            ProductionState = ProductionState.Working
        });
        baseEntity.Set(new Drawing.UI.MouseOverDisplayText
        {
            Text = "Just a base",
            RelativeSensibleArea = DefaultSpriteSize
        });
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
    public static Entity CreateMiner(World world, Vector2 location,
    int Cost = 1000,
    float Speed = 1f,
    int ItemId = 1)
    {
        var box = new Box();
        var miner = world.CreateEntity();
        var pData = new Logistic.ProvideData();
        miner.Set<IBox>(box);
        miner.Set<Production.TimeConsumption>(new Production.TimeConsumption
        {
            Cost = Cost,
            Speed = Speed,
            ProductionState = ProductionState.Working
        });
        miner.Set<Production.ItemTarget>(new Production.ItemTarget { ItemId = ItemId });
        miner.Set<Logistic.ProvideData>(pData);
        miner.Set(new Location(location));
        miner.Set(new Drawing.Sprite { Color = Color.Green, RelativeDrawingArea = DefaultSpriteSize });
        miner.Set(new Drawing.UI.MouseOverDisplayText
        {
            GetText = (Entity entity) => $"Mining {entity.Get<Production.ItemTarget>().ItemId}",
            RelativeSensibleArea = DefaultSpriteSize
        });
        return miner;
    }

    /// <summary>
    /// Creates a storage entity with access to the logistic network.
    /// </summary>
    /// <param name="world">ECS world.</param>
    /// <param name="location">Location of the entity.</param>
    /// <param name="box">If not null, the storage box.</param>
    /// <param name="requests">If not null, the requested items.</param>
    /// <param name="isProvider">If set to <see langword="true"/>, this box will be considered as provider by the LN.</param>
    public static Entity CreateStorageBox(World world, Vector2 location,
    Box? box = null,
    int[]? requests = null,
    bool isProvider = false
    )
    {
        box ??= new Box();
        requests ??= Array.Empty<int>();
        var storageBox = world.CreateEntity();
        storageBox.Set<IBox>(box);
        if (requests.Length > 0)
        {
            var req = new Logistic.RequestData();
            foreach (var item in requests)
                req.AddRequest(item, int.MaxValue);
            storageBox.Set<Logistic.RequestData>(req);
        }
        storageBox.Set<Location>(new Location(location));
        storageBox.Set(new Drawing.Sprite { Color = Color.Yellow, RelativeDrawingArea = DefaultSpriteSize });
        storageBox.Set(new Drawing.UI.MouseOverDisplayText
        {
            GetText = (Entity entity) => entity.Get<IBox>().DisplayContent(),
            RelativeSensibleArea = DefaultSpriteSize
        });
        if (isProvider)
        {
            // Add provider data, so it can provide items for the network
            storageBox.Set<Logistic.ProvideData>(new());
        }

        return storageBox;
    }
}
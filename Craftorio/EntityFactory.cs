namespace Craftorio;
using Logistic;
using Production;

/// <summary>
/// Module that exposes method for commonly used entity types.
/// </summary>
public class EntityFactory
{
    public static readonly RectangleExpand DefaultSpriteSize = new RectangleExpand
    {
        Left = 15,
        Right = 16,
        Top = 15,
        Bottom = 16,
    };

    [Newtonsoft.Json.JsonProperty("Prototypes")]
    private Dictionary<string, EntityPrototype> prototypes = new Dictionary<string, EntityPrototype>();

    public EntityFactory(World world)
    {
        World = world;
    }

    public EntityFactory(World world, string jsonFileName)
    {
        World = world;
        // Load the components field from the json file.
        var json = System.IO.File.ReadAllText(jsonFileName);
        prototypes = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, EntityPrototype>>(json, new Newtonsoft.Json.JsonSerializerSettings
        {
            TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All,
            TypeNameAssemblyFormatHandling = Newtonsoft.Json.TypeNameAssemblyFormatHandling.Simple,
            ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Replace,
            MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Error,
            Converters = new List<Newtonsoft.Json.JsonConverter>
            {
                new Craftorio.Json.ColorConverter()
            }
        }) ?? throw new System.IO.FileLoadException("Failed to load entity prototypes from json file.");
    }

    public World World { get; }

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

    public static EntityFactory CreateDefaultFactory(World world)
    {
        var factory = new EntityFactory(world);

        factory.prototypes = CreatePrototypes();

        var str = Newtonsoft.Json.JsonConvert.SerializeObject(factory.prototypes,
            Newtonsoft.Json.Formatting.Indented,
            new Newtonsoft.Json.JsonSerializerSettings
            {
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            });
        // store this string into a file
        System.IO.File.WriteAllText("prototypes.json", str);

        return factory;
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

    public Entity Create(string prototypeName)
    {
        var prototype = prototypes[prototypeName];
        var entity = prototype.CreateEntity(World);
        return entity;
    }

    /// <summary>
    /// Creates an assembler-type entity.
    /// </summary>
    /// <param name="location">Location of the entity.</param>
    /// <param name="recipe">Recipe of the assembler.</param>
    /// <param name="speed">the speed multiplier of the assembler</param>
    /// <returns>The created entity.</returns>
    public Entity CreateAssembler(Vector2 location,
        Recipe recipe,
        float speed = 1f)
    {
        var ret = Create("assembler-1");
        ret.Set(new Location(location));
        ret.Set(recipe.ToComponent());
        ret.Get<TimeConsumption>().Speed = speed;
        return ret;
    }

    public void RegisterPrototype(string name, EntityPrototype prototype)
    {
        prototypes.Add(name, prototype);
    }

    private static Dictionary<string, EntityPrototype> CreatePrototypes()
    {

        var ret = new Dictionary<string, EntityPrototype>();
        // Assembler-1
        var assembler1 = new EntityPrototype();
        ret.Add("assembler-1", assembler1);
        assembler1.AddComponent(new TimeConsumption
        {
            Cost = 0,
            Speed = 0.5f,
            ProductionState = ProductionState.Idle
        });
        assembler1.AddComponent(new Logistic.ProvideData());
        assembler1.AddComponent(new Logistic.RequestData());
        assembler1.AddComponent(new Drawing.Sprite { Color = Color.Red });
        assembler1.AddComponent(new Drawing.UI.MouseOverDisplayText
        {
            GetText = GetTooltipAssembler
        });
        assembler1.AddComponent<IBox>(new Box());

        static string GetTooltipAssembler(Entity entity)
        {
            var timer = entity.Get<TimeConsumption>();
            return $"{timer.ProductionState}#{timer.Progress}/{timer.Cost}:{entity.Get<RecipeComponent>().Outputs[0].Count}";
        }

        // Load the prototype from the file
        /*
        var str = System.IO.File.ReadAllText("prototypes.json");
        var ret = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, EntityPrototype>>(str,
            new Newtonsoft.Json.JsonSerializerSettings
            {
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            });
        */
        return ret;
    }
}
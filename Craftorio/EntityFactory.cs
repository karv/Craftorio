using System.Collections.Generic;
namespace Craftorio;
using Logistic;
using Production;

/// <summary>
/// Module that exposes method for commonly used entity types.
/// </summary>
public class EntityFactory
{
    [Newtonsoft.Json.JsonProperty("Prototypes")]
    private Dictionary<string, EntityPrototype> prototypes = new Dictionary<string, EntityPrototype>();

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityFactory"/> class.
    /// </summary>
    public EntityFactory(World world)
    {
        World = world;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityFactory"/> class using a json file with prototypes definitions.
    /// </summary>
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

    /// <summary>
    /// Gets the ECS world where the entities are created.
    /// </summary>
    /// <value></value>
    public World World { get; }

    /// <summary>
    /// Creates a default instance of this class. Currently not throwing an exception, and this method will be removed soon.
    /// </summary>
    public static EntityFactory CreateDefaultFactory(World world)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Create a new entity from a given prototype name.
    /// </summary>
    /// <returns>The created entity.</returns>
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

    /// <summary>
    /// Creates a base node entity.
    /// </summary>
    /// <param name="location">Location of the entity.</param>
    /// <param name="network">The network where this base belong.</param>
    /// <param name="carrierCount"> How many carriers are in this base.</param>
    /// <param name="constructorCount">How many constructors are in this base.</param>
    /// <returns>The created entity.</returns>
    public Entity CreateBase(Vector2 location, LogisticNetwork network,
        int carrierCount = 10,
        int constructorCount = 10)
    {
        var ret = Create("base-1");
        ret.Set(new Location(location));
        ref var be = ref ret.Get<Logistic.NodeBase>();
        be.Network = network;
        be.CarrierCount = carrierCount;
        be.ConstructorCount = constructorCount;
        return ret;
    }

    /// <summary>
    /// Creates a construction site entity.
    /// </summary>
    /// <param name="location">Location of the site.</param>
    /// <param name="prototypeName">Name of the prototype of the finished building.</param>
    /// <param name="resourcesRequired">Resources required to complete.</param>
    /// <param name="timeRequired">Construction time in milliseconds.</param>
    /// <returns>The created construction site.</returns>
    public Entity CreateConstruction(
        Vector2 location,
        string prototypeName,
        Dictionary<string, int> resourcesRequired,
        int timeRequired)
    {
        return CreateConstruction(location, this.prototypes[prototypeName], resourcesRequired, timeRequired);
    }

    /// <summary>
    /// Creates a construction site entity.
    /// </summary>
    /// <param name="location">Location of the site.</param>
    /// <param name="prototype">Prototype of the finished building.</param>
    /// <param name="resourcesRequired">Resources required to complete.</param>
    /// <param name="timeRequired">Construction time in milliseconds.</param>
    /// <returns>The created construction site.</returns>
    public Entity CreateConstruction(
        Vector2 location,
        Craftorio.EntityPrototype prototype,
        Dictionary<string, int> resourcesRequired,
        int timeRequired)
    {
        var ret = World.CreateEntity();
        ret.Set(new Location(location));
        ret.Set(new Construction.Constructing
        {
            Prototype = prototype,
            RequiredResources = resourcesRequired,
            RequiredTime = timeRequired
        });

        // Get the size from the prototype
        var protoSprite = (Drawing.Sprite)prototype.GetDictionary()[typeof(Drawing.Sprite)];

        ret.Set(new Craftorio.Drawing.Sprite
        {
            Color = Color.DarkCyan,
            RelativeDrawingArea = protoSprite.RelativeDrawingArea
        });

        ret.Set<IBox>(new Box(int.MaxValue));

        // request the resources
        var logisticRequirements = new Logistic.RequestData();
        foreach (var (key, value) in resourcesRequired)
            logisticRequirements.AddRequest(key, value);
        ret.Set(logisticRequirements);

        return ret;
    }

    /// <summary>
    /// Creates a miner entity.
    /// </summary>
    /// <param name="location">Location of the entity.</param>
    /// <param name="ItemId">Item that is mined.</param>
    public Entity CreateMiner(Vector2 location, string ItemId)
    {
        var ret = Create("miner-1");
        ret.Set(new Location(location));
        ret.Get<Production.ItemTarget>().ItemId = ItemId;

        return ret;
    }

    /// <summary>
    /// Creates a storage entity with access to the logistic network.
    /// </summary>
    /// <param name="location">Location of the entity.</param>
    /// <param name="box">If not null, the storage box.</param>
    /// <param name="requests">If not null, the requested items.</param>
    /// <param name="isProvider">If set to <see langword="true"/>, this box will be considered as provider by the LN.</param>
    public Entity CreateStorageBox(Vector2 location,
    IBox? box = null,
    string[]? requests = null,
    bool isProvider = false
    )
    {
        var ret = Create("box-1");
        if (box is null)
            box = ret.Get<IBox>();
        else
            ret.Set<IBox>(box);

        if (!isProvider) ret.Remove<Logistic.ProvideData>();

        requests ??= Array.Empty<string>();
        if (requests.Length > 0)
        {
            var req = new Logistic.RequestData();
            foreach (var item in requests)
                req.AddRequest(item, int.MaxValue);
            ret.Set<Logistic.RequestData>(req);
        }
        ret.Set<Location>(new Location(location));
        ret.Get<Drawing.UI.MouseOverDisplayText>().GetText = (Entity entity) => entity.Get<IBox>().GetDisplayContent();

        return ret;
    }

    /// <summary>
    /// Register a prototype with a specified name.
    /// </summary>
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
        return ret;
    }
}
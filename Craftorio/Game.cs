namespace Craftorio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// The main MonoGame class.
/// </summary>
public class Game : Microsoft.Xna.Framework.Game
{
    private DefaultEcs.System.ISystem<int>? drawSystem;
    private GraphicsDeviceManager graphics;
    private Logistic.LogisticNetwork network;
    private DefaultEcs.System.ISystem<int>? updateSystem;

    /// <summary>
    /// Initializes a new instance of the <see cref="Game"/> class.
    /// </summary>
    public Game()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        World = new World();
        network = new(World);

        // Set the world's components
        World.Set<UiState>();
    }

    /// <summary>
    /// The game ECS world.
    /// </summary>
    public World World { get; private set; }

    /// <summary>
    /// Draw the state of the game.
    /// </summary>
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.DimGray);
        drawSystem!.Update(gameTime.ElapsedGameTime.Milliseconds);
    }

    /// <summary>
    /// Initialize the game.
    /// </summary>
    protected override void Initialize()
    {
        SetupServices();
        base.Initialize();
        InitializeSystems();

        // Set the camera
        var camera = new MonoGame.Extended.OrthographicCamera(GraphicsDevice);
        camera.LookAt(new(0, 0));
        World.Set(camera);
        SetupInitialState(World);
    }

    /// <summary>
    /// Load the sprites.
    /// </summary>
    protected override void LoadContent()
    {
    }

    /// <summary>
    /// Update the state of the game.
    /// </summary>
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        updateSystem!.Update((int)gameTime.ElapsedGameTime.TotalMilliseconds);
    }

    private void InitializeSystems()
    {
        updateSystem = new DefaultEcs.System.SequentialSystem<int>(
            new KeyboardCameraMovement(World),
            new Production.TimeConsumingSystem(World),
            new Production.MiningSystem(World),
            new MovingObjectSystem(World),
            new ForceMouseLocationSystem(World),
            new Production.AssemblerProductionSystem(World),
            new Construction.ConstructorDeploySystem(World),
            new Logistic.BaseNodeCarrierCreationSystem(World),
            new Construction.ConstructionFinishedSystem(World),
            new Construction.ConstructionGatherSystem(World),
            new Construction.ConstructorArriveSystem(World),
            new Logistic.CarrierExecuteSystem(World),
            new Drawing.UI.DisplayTextGenerator(World) { Font = Content.Load<SpriteFont>("Fonts/Default") }
        );

        drawSystem = new DefaultEcs.System.SequentialSystem<int>(
            new Drawing.SpriteDrawing(World, GraphicsDevice),
            new Drawing.TextDrawing(World, GraphicsDevice)
        );
    }

    private void SetBuildingState(EntityPrototype prototype)
    {
        // Add an entity representing the building marker
        var entity = World.CreateEntity();
        entity.Set<Location>();
        entity.Set<ForceMouseLocation>(); // The location is forced by the mouse.
        entity.Set(new Drawing.Sprite
        {
            Color = Color.Purple * 0.4f,
            // Copy the drawing area from the prototype
            RelativeDrawingArea = prototype.GetComponent<Drawing.Sprite>().RelativeDrawingArea
        });

        // Tell the world we are building something
        World.Get<UiState>().SelectedEntityPrototypeForConstruction = prototype;
    }

    private void SetupInitialState(World World)
    {
        var factory = Services.GetService<EntityFactory>();

        // Induce the building state with "miner-1" prototype
        SetBuildingState(factory.GetPrototype("miner-1"));

        // Add a construction site for a miner.
        factory.CreateConstruction(new Vector2(-80, 0), "miner-1", new Dictionary<string, int> { { "1", 1 }, { "2", 1 } }, 5000);

        // Add some miners with different speeds and targets
        factory.CreateMiner(new(-20, 20), "1");
        factory.CreateMiner(new(0, 20), "2");
        factory.CreateMiner(new(20, 20), "3");
        factory.CreateMiner(new(40, 20), "4");

        // Add a box that ask for item 1 and 2
        factory.CreateStorageBox(new(100, 100), requests: new string[] { "10" });

        // A node so things work
        factory.CreateBase(new(0, 0), network,
            carrierCount: 3,
            constructorCount: 3);

        // An assembler that transforms itemId 1 to itemId 5 every 1 second
        var recipe = new Production.Recipe
        {
            BaseTime = 3000,
            Inputs = new ItemStack[] { new ItemStack { ItemId = "1", Count = 1 } },
            Outputs = new ItemStack[] { new ItemStack { ItemId = "5", Count = 1 } }
        };
        var asm = factory.CreateAssembler(new(-100, 100), recipe);

        // Listen to events
        // World.Subscribe<Logistic.CarrierCreated>(When);
        // World.Subscribe<Production.ProductionCompleted>(When);
        // World.Subscribe<Production.ProductionStateChanged>(When);
    }

    private void SetupServices()
    {
        //Services.AddService<EntityFactory>(EntityFactory.CreateDefaultFactory(World));
        Services.AddService<EntityFactory>(new EntityFactory(World, "prototypes.json"));
    }

    private void When(in Logistic.CarrierCreated msg)
    {
        var data = msg.Carrier.Get<Logistic.CarrierData>();

        Console.WriteLine($"Carrier created: {msg.Carrier}. {data.Order} by {msg.BaseNode}");
    }

    private void When(in Production.ProductionCompleted msg)
    {
        var box = msg.ProducerEntity.Get<IBox>();
        Console.WriteLine($"Production completed by: {msg.ProducerEntity}. Current box content: {box.GetDisplayContent()}");
    }

    private void When(in Production.ProductionStateChanged msg)
    {
        Console.WriteLine($"Production state of {msg.ProducerEntity} changed. Now is {msg.ProducerEntity.Get<Production.TimeConsumption>().ProductionState}");
    }
}
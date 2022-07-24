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
        network.Update();
    }

    private void InitializeSystems()
    {
        updateSystem = new DefaultEcs.System.SequentialSystem<int>(
            new Production.TimeConsumingSystem(World),
            new Production.MiningSystem(World),
            new MovingObjectSystem(World),
            new Production.AssemblerProductionSystem(World),
            new Logistic.CarrierExecuteSystem(World),
            new Drawing.UI.DisplayTextGenerator(World) { Font = Content.Load<SpriteFont>("Fonts/Default") }
        );
        drawSystem = new DefaultEcs.System.SequentialSystem<int>(
            new Drawing.SpriteDrawing(World, GraphicsDevice),
            new Drawing.TextDrawing(World, GraphicsDevice)
        );
    }

    private void SetupInitialState(World World)
    {
        // Add some miners with different speeds and targets
        EntityFactory.CreateMiner(World, new(-20, 20, 20, 20), Cost: 1000, Speed: 1, ItemId: 1);
        EntityFactory.CreateMiner(World, new(0, 20, 20, 20), Cost: 1000, Speed: 1.3f, ItemId: 2);
        EntityFactory.CreateMiner(World, new(20, 20, 20, 20), Cost: 1000, Speed: 1.5f, ItemId: 3);
        EntityFactory.CreateMiner(World, new(40, 20, 20, 20), Cost: 1000, Speed: 2f, ItemId: 4);

        // Add a box that ask for item 1 and 2
        EntityFactory.CreateStorageBox(World, new(100, 100, 20, 20), requests: new[] { 1, 2 });

        // A node so things work
        EntityFactory.CreateBase(World, new(0, 0, 20, 20));

        // An assembler that transforms itemId 1 to itemId 5 every 1 second
        var recipe = new Production.Recipe
        {
            BaseTime = 1000,
            Inputs = new ItemStack[] { new ItemStack { ItemId = 1, Count = 1 } },
            Outputs = new ItemStack[] { new ItemStack { ItemId = 5, Count = 1 } }
        };
        var asm = EntityFactory.CreateAssembler(World, new(-100, 100, 20, 20), recipe, includeLogisticSupport: true);
        // Add some items to the assembler
        Box box = (Box)asm.Get<ITakeableBox>();
        box.TryStore(1, 10);

        // Listen to carrier created events
        World.Subscribe<Logistic.CarrierCreated>(When);
        World.Subscribe<Production.ProductionCompleted>(When);
        World.Subscribe<Production.ProductionStateChanged>(When);

    }

    private void SetupServices()
    {
    }

    private void When(in Logistic.CarrierCreated msg)
    {
        var data = msg.Carrier.Get<Logistic.CarrierData>();

        Console.WriteLine($"Carrier created: {msg.Carrier}. {data.Order}");
    }

    private void When(in Production.ProductionCompleted msg)
    {
        var box = msg.ProducerEntity.Get<IStoreBox>();
        Console.WriteLine($"Production completed by: {msg.ProducerEntity}. Current box content: {box.DisplayContent()}");
    }

    private void When(in Production.ProductionStateChanged msg)
    {
        Console.WriteLine($"Production state of {msg.ProducerEntity} changed. Now is {msg.ProducerEntity.Get<Production.TimeConsumption>().ProductionState}");
    }
}
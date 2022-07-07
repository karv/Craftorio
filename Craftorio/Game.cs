namespace Craftorio;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
public class Game : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Logistic.LogisticNetwork network;
    private DefaultEcs.System.ISystem<int> updateSystem;

    public Game()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        World = new World();
        updateSystem = new DefaultEcs.System.SequentialSystem<int>(
            new Production.TimeConsumingSystem(World),
            new Production.MiningSystem(World),
            new MovingObjectSystem(World),
            new Production.AssemblerProductionSystem(World),
            new Logistic.CarrierExecuteSystem(World)
        );
        network = new(World);
    }
    public World World { get; private set; }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        base.Draw(gameTime);
    }

    protected override void Initialize()
    {
        base.Initialize();

        // Add some miners with different speeds and targets
        EntityFactory.CreateMiner(World, new Vector2(-20, 20), Cost: 1000, Speed: 1, ItemId: 1);
        EntityFactory.CreateMiner(World, new Vector2(0, 20), Cost: 1000, Speed: 1.3f, ItemId: 2);
        EntityFactory.CreateMiner(World, new Vector2(20, 20), Cost: 1000, Speed: 1.5f, ItemId: 3);
        EntityFactory.CreateMiner(World, new Vector2(40, 20), Cost: 1000, Speed: 2f, ItemId: 4);

        // Add a box that ask for item 1 and 2
        EntityFactory.CreateStorageBox(World, new(100, 100), requests: new[] { 1, 2 });

        // A node so things work
        EntityFactory.CreateBase(World, new(0, 0));

        // An assembler that transforms itemId 1 to itemId 5 every 1 second
        var recipe = new Production.Recipe
        {
            BaseTime = 1000,
            Inputs = new ItemStack[] { new ItemStack { ItemId = 1, Count = 1 } },
            Outputs = new ItemStack[] { new ItemStack { ItemId = 5, Count = 1 } }
        };
        var asm = EntityFactory.CreateAssembler(World, new(-100, 100), recipe, includeLogisticSupport: true);
        // Add some items to the assembler
        Box box = (Box)asm.Get<ITakeableBox>();
        box.TryStore(1, 10);

        // Listen to carrier created events
        // World.Subscribe<Logistic.CarrierCreated>(When);
        World.Subscribe<Production.ProductionCompleted>(When);
        World.Subscribe<Production.ProductionStateChanged>(When);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        updateSystem.Update((int)gameTime.ElapsedGameTime.TotalMilliseconds);
        network.Update();
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

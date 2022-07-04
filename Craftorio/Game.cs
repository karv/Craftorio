using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Craftorio
{
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

            // Listen to carrier created events
            World.Subscribe<Logistic.CarrierCreated>(When);
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
    }
}

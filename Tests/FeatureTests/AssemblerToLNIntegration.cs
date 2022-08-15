namespace Test;
using DefaultEcs;

[TestFixture]
public class AssemblerToLNIntegration
{
    private readonly DefaultEcs.System.ISystem<int> system;
    private readonly World world;
    private Craftorio.Logistic.LogisticNetwork network;
    // Let the system run for 5 secs at 60 FPS
    const int deltaMilliseconds = 1000 / 60;
    const int iterations = totalMilliseconds / deltaMilliseconds;
    const int totalMilliseconds = 5000;

    public AssemblerToLNIntegration()
    {
        world = new();
        network = new(world);
        system = new DefaultEcs.System.SequentialSystem<int>(
            new Craftorio.Production.TimeConsumingSystem(world),
            new Craftorio.Production.AssemblerProductionSystem(world),
            new Craftorio.Logistic.BaseNodeCarrierCreationSystem(world)
        );
    }

    [Test]
    public void AssemblersAreProviders()
    {
        var factory = Craftorio.EntityFactory.CreateDefaultFactory(world);
        // Add a assembler and a box that requests 1 output item.
        // Include the recipe requirements in the box of the assembler.
        var recipe = new Craftorio.Production.Recipe
        {
            Outputs = new[] { new ItemStack { ItemId = 1, Count = 1 } },
            Inputs = new[] { new ItemStack { ItemId = 0, Count = 1 } },
            BaseTime = 10 // 10 ms per batch
        };

        var ent0 = factory.CreateAssembler(
            new (0, 0),
            recipe);
        var ent1 = EntityFactory.CreateStorageBox(
            world,
            new Microsoft.Xna.Framework.Vector2(0, 0),
            requests: new[] { 1 });

        // Add some materials to the assembler.
        ((Box)ent0.Get<IBox>()).TryStore(0, 10);
        bool carrierCreated = false;

        // Run the system for 5 seconds, for then the assembler should have provided at least 1 item into the storage.
        // So we listen to the event
        world.Subscribe<Craftorio.Logistic.CarrierCreated>(CarrierCreated);
        for (int i = 0; i < iterations; i++)
            system.Update(deltaMilliseconds);

        Assert.That(carrierCreated, Is.True);

        void CarrierCreated(in Craftorio.Logistic.CarrierCreated e)
        {
            carrierCreated = true;
        }
    }

    [SetUp]
    public void Setup()
    {
        network = new Craftorio.Logistic.LogisticNetwork(world);
        // Create a base at origin
        EntityFactory.CreateBase(
            world,
            default,
            network);
    }
}

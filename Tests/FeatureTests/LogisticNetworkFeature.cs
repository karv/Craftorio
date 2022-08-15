namespace Test;
using DefaultEcs;

/// <summary>
/// Tests covering the logistic network feature.
/// </summary>
[TestFixture]
public class LogisticNetworkFeature
{
    private Craftorio.Logistic.LogisticNetwork logisticNetwork;
    private DefaultEcs.System.ISystem<int> system;
    private World world;

    [Test]
    public void BasicRequestedItemTransport()
    {
        var factory = Craftorio.EntityFactory.CreateDefaultFactory(world);
        // Add a provider box, a requester box, and a carrier base.
        var proBox = new Box();
        var reqBox = new Box();
        proBox.TryStore(1, 1);
        var provider = factory.CreateStorageBox(new Microsoft.Xna.Framework.Vector2(0, 0),
            box: proBox,
            isProvider: true);
        var requester = factory.CreateStorageBox(new Microsoft.Xna.Framework.Vector2(0, 0),
            box: reqBox,
            requests: new[] { 1 });
        var nodeBase = factory.CreateBase(new Microsoft.Xna.Framework.Vector2(0, 0), logisticNetwork);

        // Run for 5 seconds at 60fps
        const int deltaTime = 1000 / 60;
        for (int i = 0; i < 5 * 60; i++)
            system.Update(deltaTime);

        // The requester should have received the item, and the provider should be empty.
        Assert.That(reqBox[1], Is.EqualTo(1));
        Assert.That(proBox[1], Is.EqualTo(0));
    }

    [Test]
    public void DoNotCreateMoreOrdersThanTheRequest()
    {
        var factory = Craftorio.EntityFactory.CreateDefaultFactory(world);
        // Add a provider box, a requester box, and a carrier base.
        var proBox = new Box();
        var reqBox = new Box();
        proBox.TryStore(1, 10);

        var provider = factory.CreateStorageBox(new Microsoft.Xna.Framework.Vector2(0, 0),
            box: proBox,
            isProvider: true);
        var requester = factory.CreateStorageBox(new Microsoft.Xna.Framework.Vector2(0, 0),
            box: reqBox,
            requests: new[] { 1 });
        var nodeBase = factory.CreateBase(new Microsoft.Xna.Framework.Vector2(0, 0), logisticNetwork);

        // The requester only want 3 of the item 1.
        requester.Get<Craftorio.Logistic.RequestData>().ChangeRequestOf(1, 3);

        // Set the timer of the base to ready.
        nodeBase.Get<Craftorio.Production.TimeConsumption>().Complete();

        // Run once to let the network update.
        system.Update(0);

        // Should have 3 (or 2 if a carrier was dispatched) orders.
        Assert.That(logisticNetwork.GetAllOrdersFromTo(provider, requester).Sum(x => x.Amount), Is.AnyOf(2, 3));
    }

    [Test]
    public void NoProvidersWillNotCreateCarrier()
    {
        var factory = Craftorio.EntityFactory.CreateDefaultFactory(world);
        // Add a base and a requester
        var baseNode = factory.CreateBase(new Microsoft.Xna.Framework.Vector2(0, 0), logisticNetwork);
        var requester = factory.CreateStorageBox(new Microsoft.Xna.Framework.Vector2(0, 0),
            requests: new[] { 1 });
        // Subscribe: if any carrier is created, fail the test.
        world.Subscribe<Craftorio.Logistic.CarrierCreated>(AssertNoCarrierCreated);
        // Run for 5 seconds at 60fps
        const int deltaTime = 1000 / 60;
        for (int i = 0; i < 5 * 60; i++)
            system.Update(deltaTime);

        static void AssertNoCarrierCreated(in Craftorio.Logistic.CarrierCreated e)
        {
            Assert.Fail();
        }
    }
    [Test]
    public void NoRequestersWillNotCreateCarrier()
    {
        var factory = Craftorio.EntityFactory.CreateDefaultFactory(world);
        // Add a base and a provider
        var proBox = new Box();
        proBox.TryStore(1, 1);
        var provider = factory.CreateStorageBox(new Microsoft.Xna.Framework.Vector2(0, 0),
            box: proBox,
            isProvider: true);
        var baseNode = factory.CreateBase(new Microsoft.Xna.Framework.Vector2(0, 0), logisticNetwork);
        // Subscribe: if any carrier is created, fail the test.
        world.Subscribe<Craftorio.Logistic.CarrierCreated>(AssertNoCarrierCreated);
        // Run for 5 seconds at 60fps
        const int deltaTime = 1000 / 60;
        for (int i = 0; i < 5 * 60; i++)
            system.Update(deltaTime);

        static void AssertNoCarrierCreated(in Craftorio.Logistic.CarrierCreated e)
        {
            Assert.Fail();
        }
    }

    [SetUp]
    public void SetUp()
    {
        world = new();
        system = new DefaultEcs.System.SequentialSystem<int>(
            new Craftorio.Logistic.CarrierExecuteSystem(world),
            new Craftorio.MovingObjectSystem(world),
            new Craftorio.Logistic.BaseNodeCarrierCreationSystem(world),
            new Craftorio.Production.TimeConsumingSystem(world)
        );
        logisticNetwork = new(world);
    }
}
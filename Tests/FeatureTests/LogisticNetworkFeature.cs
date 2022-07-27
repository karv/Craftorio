namespace Test;
// Tests covering the logistic network feature.
using DefaultEcs;

public class LogisticNetworkFeature
{
    private readonly Craftorio.Logistic.LogisticNetwork logisticNetwork;
    private readonly DefaultEcs.System.ISystem<int> system;
    private readonly World world;

    public LogisticNetworkFeature()
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

    [Test]
    public void BasicRequestedItemTransport()
    {
        // Add a provider box, a requester box, and a carrier base.
        var proBox = new Box();
        var reqBox = new Box();
        const int boxDistance = 10;
        proBox.TryStore(1, 1);
        var provider = EntityFactory.CreateStorageBox(world, new MonoGame.Extended.RectangleF(0, 0, 10, 10),
            box: proBox,
            isProvider: true);
        var requester = EntityFactory.CreateStorageBox(world, new MonoGame.Extended.RectangleF(boxDistance, 0, 10, 10),
            box: reqBox,
            requests: new[] { 1 });
        var nodeBase = EntityFactory.CreateBase(world, new MonoGame.Extended.RectangleF(0, 0, 10, 10), logisticNetwork);

        // Run for 5 seconds at 60fps
        const int deltaTime = 1000 / 60;
        for (int i = 0; i < 5 * 60; i++)
        {
            system.Update(deltaTime);
        }
        // The requester should have received the item, and the provider should be empty.
        Assert.AreEqual(1, reqBox[1]);
        Assert.AreEqual(0, proBox[1]);
    }
}
namespace Test;
using DefaultEcs;

[TestFixture]
public class ItemProductionChain
{
    private readonly DefaultEcs.System.ISystem<int> system;
    private readonly World world;

    public ItemProductionChain()
    {
        world = new();
        system = new DefaultEcs.System.SequentialSystem<int>(
            new Craftorio.Production.TimeConsumingSystem(world),
            new Craftorio.Production.AssemblerProductionSystem(world),
            new Craftorio.Production.MiningSystem(world)
        );
    }

    [Test]
    public void SetupMiner()
    {
        var ent0 = EntityFactory.CreateMiner(world, new MonoGame.Extended.RectangleF(0, 0, 100, 100));
        var ent1 = EntityFactory.CreateMiner(world, new MonoGame.Extended.RectangleF(0, 0, 100, 100),
        Speed: 1.5f);
        var ent2 = EntityFactory.CreateMiner(world, new MonoGame.Extended.RectangleF(0, 0, 100, 100),
        Speed: 2f);

        // Let the system run for 5 secs at 60 FPS
        const int deltaMilliseconds = 1000 / 60;
        const int totalMilliseconds = 5000;
        const int iterations = totalMilliseconds / deltaMilliseconds
         + 10; // Add 10 for the lost on cycle finishing for each expected completed item in the last assertion. ;
        for (int i = 0; i < iterations; i++)
            system.Update(deltaMilliseconds);

        // The system state should be:
        // - Miners should be mining
        // - Miner0 should have mined 5 items
        // - Miner1 should have mined 7 items
        // - Miner2 should have mined 10 items

        Assert.AreEqual(5, ent0.Get<ITakeableBox>()[1]);
        Assert.AreEqual(7, ent1.Get<ITakeableBox>()[1]);
        Assert.AreEqual(10, ent2.Get<ITakeableBox>()[1]);
    }
}
namespace Test;
using DefaultEcs;

[TestFixture]
public class ItemProductionChain
{
    private readonly DefaultEcs.System.ISystem<int> system;
    private readonly World world;

    // Let the system run for 5 secs at 60 FPS
    const int deltaMilliseconds = 1000 / 60;
    const int iterations = totalMilliseconds / deltaMilliseconds;
    const int totalMilliseconds = 5000;

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
    public void SetupAssembler()
    {
        var factory = Craftorio.EntityFactory.CreateDefaultFactory(world);
        var recipe = new Craftorio.Production.Recipe
        {
            Outputs = new[] { new ItemStack { ItemId = "1", Count = 1 } },
            Inputs = new[] { new ItemStack { ItemId = "0", Count = 1 } },
            BaseTime = 10 // 10 ms per batch
        };
        var ent0 = factory.CreateAssembler(new(0, 0), recipe);
        // Add some materials to the assembler.
        Box input = (Box)ent0.Get<IBox>();
        input.TryStore("0", 10);

        for (int i = 0; i < iterations; i++)
            system.Update(deltaMilliseconds);

        // All materials should be consumed, and the output should be produced.
        Assert.That(input["0"], Is.EqualTo(0));
        Box output = (Box)ent0.Get<IBox>();
        Assert.That(output["1"], Is.EqualTo(10));

        // the production state of the assembler should be "waiting for resources" again.
        Assert.That(ent0.Get<Craftorio.Production.TimeConsumption>().ProductionState,
            Is.EqualTo(Craftorio.Production.ProductionState.WaitingForResources));
    }

    [Test]
    public void SetupMiner()
    {
        var factory = Craftorio.EntityFactory.CreateDefaultFactory(world);
        var ent0 = factory.CreateMiner(new Microsoft.Xna.Framework.Vector2(0, 0), "1");
        var ent1 = factory.CreateMiner(new Microsoft.Xna.Framework.Vector2(0, 0), "1");
        ent1.Get<Craftorio.Production.TimeConsumption>().Speed = 1.5f;
        var ent2 = factory.CreateMiner(new Microsoft.Xna.Framework.Vector2(0, 0), "1");
        ent1.Get<Craftorio.Production.TimeConsumption>().Speed = 2f;

        var iterations = ItemProductionChain.iterations + 10; // Add 10 for the lost on cycle finishing for each expected completed item in the last assertion. ;
        for (int i = 0; i < iterations; i++)
            system.Update(deltaMilliseconds);

        // The system state should be:
        // - Miners should be mining
        // - Miner0 should have mined 5 items
        // - Miner1 should have mined 7 items
        // - Miner2 should have mined 10 items

        Assert.That(ent0.Get<IBox>()["1"], Is.EqualTo(5));
        Assert.That(ent1.Get<IBox>()["1"], Is.EqualTo(7));
        Assert.That(ent2.Get<IBox>()["1"], Is.EqualTo(10));
    }
}
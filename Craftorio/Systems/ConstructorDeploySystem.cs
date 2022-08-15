namespace Craftorio.Construction;
using DefaultEcs.System;
using Production;

/// <summary>
/// Deploys constructors when there is a construction site.
/// </summary>
public sealed class ConstructorDeploySystem : AEntitySetSystem<int>
{

    private static readonly RectangleExpand constructorSize = new RectangleExpand
    {
        Left = 5,
        Right = 5,
        Top = 5,
        Bottom = 5
    };
    /// <summary>
    /// Entities considered in step 2 (construction) of the construction process.
    /// </summary>
    private readonly EntitySet constructingEntities;

    private Entity targetConstructionSite;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConstructorDeploySystem"/> class.
    /// </summary>
    public ConstructorDeploySystem(World world) :
        base(world.GetEntities()
        .With<Logistic.NodeBase>()
        .With<TimeConsumption>()
        .With<Location>()
        .AsSet())
    {
        constructingEntities = world.GetEntities()
            .With<Constructing>()
            .With<Location>()
            .With<TimeConsumption>()
            .AsSet();
    }

    /// <summary>
    /// Check whether there is a construction site that needs a constructor,
    /// and if so, deploy a constructor to it using the <see cref="Update(int, in Entity)"/> method
    /// </summary>
    protected override void Update(int state, ReadOnlySpan<Entity> entities)
    {
        // Do nothing if no entities are under construction.
        if (constructingEntities.Count == 0)
            return;
        targetConstructionSite = constructingEntities.GetEntities()[0];
        base.Update(state, entities);
    }

    /// <summary>
    /// Deploy the specified constructor (<paramref name="entity"/>) to the target construction site
    /// defined by the <see cref="Update(int, ReadOnlySpan{Entity})"/> method.
    /// </summary>
    /// <param name="state"></param>
    /// <param name="entity"></param>
    protected override void Update(int state, in Entity entity)
    {
        // here entity is a base node.
        // If the timer is not completed, or if no constructors exist return
        ref var timer = ref entity.Get<TimeConsumption>();
        ref var node = ref entity.Get<Logistic.NodeBase>();
        if (!timer.IsCompleted || node.ConstructorCount == 0)
            return;

        timer.Reset();

        // Deploy a constructor to the first entity in the constructingEntities set.
        var constructor = CreateConstructor(World, entity.Get<Location>(), node.Network);
        node.ConstructorCount--;

        // Tell the constructor to move towards the construction site.
        constructor.Set(new MovingObject
        {
            TargetEntity = targetConstructionSite,
            MoveSpeed = node.Network.CarriersSpeed
        });
    }

    private static Entity CreateConstructor(World world, in Location location, Logistic.LogisticNetwork network)
    {
        var constructor = world.CreateEntity();
        constructor.Set(location);
        constructor.Set(new Logistic.ConstructorData
        {
            ConstructionSpeed = 1f,
            Network = network,
            State = Logistic.ConstructorData.ConstructorState.GoToConstructionSite
        });
        constructor.Set(new Craftorio.Drawing.Sprite
        {
            Color = Color.LightBlue,
            RelativeDrawingArea = constructorSize
        });

        return constructor;
    }
}
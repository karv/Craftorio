namespace Craftorio.Logistic;
using DefaultEcs.System;
using Production;

/// <summary>
/// This system creates carriers nodes for every base node that requires.
/// </summary>
public class BaseNodeCarrierCreationSystem : AEntitySetSystem<int>
{

    private static readonly RectangleExpand carrierSize = new RectangleExpand
    {
        Left = 5,
        Right = 5,
        Top = 5,
        Bottom = 5
    };
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseNodeCarrierCreationSystem"/> class.
    /// </summary>
    public BaseNodeCarrierCreationSystem(World world) : base(world.GetEntities()
        .With<NodeBase>()
        .With<TimeConsumption>()
        .With<Location>()
        .AsSet())
    {
    }

    /// <summary>
    /// Updates a specified node base entity. If this entity requires a carrier node, it will be created.
    /// </summary>
    protected override void Update(int state, in Entity entity)
    {
        ref var timer = ref entity.Get<TimeConsumption>();
        // if the timer is not completed, return
        if (!timer.IsCompleted)
            return;

        // If the order queue is empty, ask the network to assign orders.
        ref var nodeBase = ref entity.Get<NodeBase>();

        // If no available carriers, return.
        if (nodeBase.CarrierCount == 0)
            return;

        ref var location = ref entity.Get<Location>();

        // Take the first order from the queue, if any,
        // and assign it to the carrier.
        if (nodeBase.OrdersQueue.Count > 0)
        {
            // required use of ref, because the amount may change during the process.
            ref var order = ref nodeBase.OrdersQueue.Peek();
            LogisticOrder carrierOrder = order with { Amount = 1 };

            if (order.Amount == 1)
                nodeBase.OrdersQueue.Dequeue();
            else order.Amount--;

            var carrier = CreateCarrier(order, nodeBase.Network, location, entity);
            nodeBase.CarrierCount--; // Remove one carrier from the base.
        }
        else
            nodeBase.Network.AssignOrders();

        // Reset the timer.
        timer.Reset();
    }

    private Entity CreateCarrier(LogisticOrder order, LogisticNetwork network, in Location location, in Entity baseNode)
    {
        var carrier = World.CreateEntity();
        // Put it somewhere, at the origin for now
        carrier.Set(in location);
        carrier.Set(new CarrierData
        {
            Order = order,
            State = CarrierState.Requesting,
            Content = new ItemStack { Count = 0, ItemId = order.ItemId },
            Network = network
        });
        carrier.Set(new MovingObject
        {
            MoveSpeed = network.CarriersSpeed,
            TargetEntity = order.SourceNode
        });
        carrier.Set(new Craftorio.Drawing.Sprite
        {
            Color = Color.YellowGreen,
            RelativeDrawingArea = carrierSize,
        });
        World.Publish(new CarrierCreated { Carrier = carrier, BaseNode = baseNode });
        return carrier;
    }
}
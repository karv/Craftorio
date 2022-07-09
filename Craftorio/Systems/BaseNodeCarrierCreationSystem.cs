namespace Craftorio.Logistic;
using DefaultEcs.System;
using Production;

public class BaseNodeCarrierCreationSystem : AEntitySetSystem<int>
{
    public BaseNodeCarrierCreationSystem(World world) : base(world.GetEntities()
        .With<NodeBase>()
        .With<TimeConsumption>()
        .With<Location>()
        .AsSet())
    {
    }

    protected override void Update(int state, in Entity entity)
    {
        ref var timer = ref entity.Get<TimeConsumption>();
        // if the timer is not completed, return
        if (!timer.IsCompleted)
            return;

        // If the order queue is empty, ask the network to assign orders.
        ref var nodeBase = ref entity.Get<NodeBase>();
        ref var location = ref entity.Get<Location>();

        // Take the first order from the queue, if any,
        // and assign it to the carrier.
        if (nodeBase.OrdersQueue.TryPeek(out var order))
        {
            LogisticOrder carrierOrder = order with { Amount = 1 };

            if (order.Amount == 1)
                nodeBase.OrdersQueue.Dequeue();

            var carrier = CreateCarrier(order, nodeBase.Network, location.AsVector, entity);
        }
        else
            nodeBase.Network.AssignOrders();

        // Reset the timer.
        timer.Reset();
    }

    private Entity CreateCarrier(LogisticOrder order, LogisticNetwork network, Vector2 location, Entity baseNode)
    {
        var carrier = World.CreateEntity();
        // Put it somewhere, at the origin for now
        carrier.Set<Location>(new Location { AsVector = location });
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
        World.Publish(new CarrierCreated { Carrier = carrier, BaseNode = baseNode });
        return carrier;
    }
}
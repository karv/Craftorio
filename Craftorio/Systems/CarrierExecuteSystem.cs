namespace Craftorio.Logistic;
using System.Runtime.CompilerServices;
using DefaultEcs.System;

/// <summary>
/// Controls the behavior of the carriers, by updating its state in function of its fixed order.
/// </summary>
public sealed class CarrierExecuteSystem : AEntitySetSystem<int>
{
    /// <summary>
    /// Recorder to destroy the entity when they complete their work.
    /// </summary>
    private DefaultEcs.Command.EntityCommandRecorder destroyingRecords;

    /// <summary>
    /// Initializes a new instance of the <see cref="CarrierExecuteSystem"/> class.
    /// </summary>
    public CarrierExecuteSystem(World world) :
        base(world.GetEntities()
            .With<CarrierData>()
            .With<Location>()
            .With<MovingObject>()
            .AsSet())
    {
        destroyingRecords = new DefaultEcs.Command.EntityCommandRecorder();
    }

    /// <summary>
    /// Destroy the carries when they complete their work.
    /// </summary>
    protected override void PostUpdate(int state)
    {
        // Actually destroy the entities in the destroyingRecords.
        destroyingRecords.Execute();
    }

    /// <summary>
    /// Update the state of a specified carrier entity, depending on its fixed order.
    /// </summary>
    protected override void Update(int state, in Entity entity)
    {
        ref var data = ref entity.Get<CarrierData>();
        switch (data.State)
        {
            case CarrierState.Requesting:
                UpdateRequesting(in entity, ref data, state);
                break;
            case CarrierState.Delivering:
                UpdateDelivering(in entity, ref data, state);
                break;
            case CarrierState.Returning:
                UpdateReturning(in entity, ref data, state);
                break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateDelivering(in Entity carrier, ref CarrierData data, int elapsedMilliseconds)
    {
        ref var location = ref carrier.Get<Location>();
        ref var movingObject = ref carrier.Get<MovingObject>();
        ref var target = ref movingObject.TargetEntity.Get<Location>();
        var tickDisplacement = (int)movingObject.MoveSpeed * elapsedMilliseconds;
        // If the distance is less than the tick displacement, we are there.
        var distance = (location.AsVector - target.AsVector).Length();
        if (distance <= tickDisplacement)
        {
            // Drop the cargo into the target node.
            var box = movingObject.TargetEntity.Get<IStoreBox>();
            box.TryStore(data.Content);
            data.Content = default; // Remove the content from the carrier.

            // Update the requester status
            var requester = movingObject.TargetEntity.Get<RequestData>();
            requester.ChangeCurrentOrders(data.Order.ItemId, -data.Order.Amount);

            // Update the carrier state.
            data.State = CarrierState.Returning;

            // Return to any base node.
            data.Network.TryGetClosestBase(location.AsVector, out movingObject.TargetEntity);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateRequesting(in Entity carrier, ref CarrierData data, int elapsedMilliseconds)
    {
        // At this moment we have the target node where to pick up the cargo.
        // Other system will update the location, we only have to determine whether we are there or not.

        ref var location = ref carrier.Get<Location>();
        ref var movingObject = ref carrier.Get<MovingObject>();
        var target = movingObject.TargetEntity.Get<Location>();
        var tickDisplacement = (int)movingObject.MoveSpeed * elapsedMilliseconds;
        // If the distance is less than the tick displacement, we are there.
        var distance = (location.AsVector - target.AsVector).Length();
        if (distance <= tickDisplacement)
        {
            // We are there, we can pick up the cargo, raise the flag?, update the state and return.
            data.State = CarrierState.Delivering;
            // Pick the box from the target of the movement component
            var box = movingObject.TargetEntity.Get<ITakeableBox>();
            data.Content = box.Take(data.Order.ItemId, data.Order.Amount);

            // Update the provider status
            var provider = movingObject.TargetEntity.Get<ProvideData>();
            provider.ChangeCurrentOrders(data.Order.ItemId, -data.Order.Amount);

            // Set the target entity to the target node of the order, since we are moving there
            // to put in the content.

            movingObject.TargetEntity = data.Order.DestinationNode;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateReturning(in Entity carrier, ref CarrierData data, int elapsedMilliseconds)
    {
        ref var location = ref carrier.Get<Location>();
        ref var movingObject = ref carrier.Get<MovingObject>();
        ref var target = ref movingObject.TargetEntity.Get<Location>();
        var tickDisplacement = (int)movingObject.MoveSpeed * elapsedMilliseconds;
        // If the distance is less than the tick displacement, we are there.
        var distance = (location.AsVector - target.AsVector).Length();
        if (distance <= tickDisplacement)
        {
            // Destroy this carrier and increase the available carriers.
            data.Network.AvailableCarrierCount++;
            destroyingRecords.Record(carrier).Dispose();
        }
    }
}
namespace Craftorio.Logistic;

/// <summary>
/// Contains information on how a carrier must move.
/// </summary>
public record struct LogisticOrder
{
    /// <summary>
    /// Amount of items to move.
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    /// The destination entity.
    /// </summary>
    public Entity DestinationNode { get; init; }

    /// <summary>
    /// ID of the item to move.
    /// </summary>
    public string ItemId { get; init; }

    /// <summary>
    /// The source entity.
    /// </summary>
    public Entity SourceNode { get; init; }

    /// <summary>
    /// Cancel this order. This will update the network context.
    /// </summary>
    public void Cancel()
    {
        if (!SourceNode.IsAlive || !DestinationNode.IsAlive) return;
        SourceNode.Get<ProvideData>().ChangeCurrentOrders(ItemId, -Amount);
        DestinationNode.Get<RequestData>().ChangeCurrentOrders(ItemId, -Amount);
    }

    /// <summary>
    /// Return a string representation of this order.
    /// </summary>
    public override string ToString()
    {
        return $"{SourceNode.Get<Location>()} -> {DestinationNode.Get<Location>()} : {Amount}*{ItemId}";
    }
}
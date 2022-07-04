namespace Craftorio.Logistic;

public struct LogisticOrder
{
    public int Amount { get; set; }
    public Entity DestinationNode { get; init; }
    public int ItemId { get; init; }
    public Entity SourceNode { get; init; }
    public void Cancel()
    {
        if (!SourceNode.IsAlive || !DestinationNode.IsAlive) return;
        SourceNode.Get<ProvideData>().OnTheWayOrders[ItemId] -= Amount;
        DestinationNode.Get<RequestData>().OnTheWayOrders[ItemId] -= Amount;
    }

    // Update the logistic state for both provider and requester.
    public void Complete()
    {
        SourceNode.Get<ProvideData>().OnTheWayOrders[ItemId] -= Amount;
        DestinationNode.Get<RequestData>().OnTheWayOrders[ItemId] -= Amount;
    }

    public override string ToString()
    {
        return $"{SourceNode.Get<Location>()} -> {DestinationNode.Get<Location>()} : {Amount}*{ItemId}";
    }
}
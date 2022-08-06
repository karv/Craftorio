namespace Craftorio.Logistic;

/// <summary>
/// A component unique for bases.
/// </summary>
public record struct NodeBase
{
    /// <summary>
    /// Capacity of carriers storage.
    /// </summary>
    public int CarrierCount;

    /// <summary>
    /// the queue of pending orders for the carriers.
    /// </summary>
    public CE.Collections.Queue<LogisticOrder> OrdersQueue;

    /// <summary>
    /// The network instance that this base belongs to.
    /// </summary>
    public LogisticNetwork Network;
}
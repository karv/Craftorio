namespace Craftorio.Logistic;

/// <summary>
/// A component unique for bases.
/// </summary>
public record struct NodeBase
{
    /// <summary>
    /// Capacity of carriers storage.
    /// </summary>
    public int Capacity;

    public Queue<LogisticOrder> OrdersQueue;
    public LogisticNetwork Network;
}
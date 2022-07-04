namespace Craftorio.Logistic;

/// <summary>
/// Stores, updates and manages logistic orders for consumption of the <see cref="LogisticNetwork"/>.
/// </summary>
public class LogisticOrdersManager
{
    private const int ordersBufferSize = 1024;
    private int nextOrderIndex = 0;
    private LogisticOrder[] orders = new LogisticOrder[ordersBufferSize];
    private int ordersCount = 0;

    /// <summary>
    /// Determines whether the orders buffer is empty.
    /// </summary>
    public bool IsEmpty => nextOrderIndex == ordersCount;

    /// <summary>
    /// Gets the full span of the orders buffer. This includes noise orders.
    /// </summary>
    public Span<LogisticOrder> AsSpan()
    {
        return orders.AsSpan();
    }

    /// <summary>
    /// Cancel all orders and clear the orders queue.
    /// </summary>
    public void CancelAndClear()
    {
        foreach (var order in QueuedAsSpan())
            order.Cancel();
        Clear();
    }

    /// <summary>
    /// Clear the orders queue.
    /// </summary>
    public void Clear()
    {
        AsSpan().Fill(default);
        nextOrderIndex = 0;
        ordersCount = 0;
    }

    /// <summary>
    /// Gets the next order in the queue
    /// </summary>
    public ref LogisticOrder GetNextOrder()
    {
        if (ordersCount == ordersBufferSize)
            throw new InvalidOperationException("No more orders available");
        return ref orders[nextOrderIndex++];
    }

    /// <summary>
    /// Gets a span containing the orders in the queue.
    /// </summary>
    public Span<LogisticOrder> QueuedAsSpan()
    {
        return orders.AsSpan(nextOrderIndex);
    }

    /// <summary>
    /// This method is exclusive for the LogisticNetwork class, used when rebuffing.
    /// </summary>
    public void SetCount(int count)
    {
        ordersCount = count;
    }

    /// <summary>
    /// Picks up the next order in the queue, given the maximum amount of items to pick up.
    /// </summary>
    public bool TryDequeue(int amount, out LogisticOrder order)
    {
        if (nextOrderIndex == ordersCount)
        {
            order = default;
            return false;
        }
        var innerOrder = orders[nextOrderIndex];
        amount = Math.Min(amount, innerOrder.Amount);
        innerOrder.Amount -= amount;
        if (innerOrder.Amount == 0)
            nextOrderIndex++;
        order = new LogisticOrder
        {
            Amount = amount,
            DestinationNode = innerOrder.DestinationNode,
            SourceNode = innerOrder.SourceNode,
            ItemId = innerOrder.ItemId
        };
        return true;
    }
}
namespace Craftorio.Logistic;

public class LogisticOrdersManager
{
    private const int ordersBufferSize = 1024;
    private int nextOrderIndex = 0;
    private LogisticOrder[] orders = new LogisticOrder[ordersBufferSize];
    private int ordersCount = 0;

    public bool IsEmpty => nextOrderIndex == ordersCount;

    public Span<LogisticOrder> AsSpan()
    {
        return orders.AsSpan();
    }

    public void CancelAndClear()
    {
        foreach (var order in QueuedAsSpan())
            order.Cancel();
        Clear();
    }

    public void Clear()
    {
        AsSpan().Fill(default);
        nextOrderIndex = 0;
        ordersCount = 0;
    }
    public ref LogisticOrder GetNextOrder()
    {
        if (ordersCount == ordersBufferSize)
            throw new InvalidOperationException("No more orders available");
        return ref orders[nextOrderIndex++];
    }

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
namespace Craftorio.Logistic;

// This should be struct, but when there is a struct for the OnTheWayOrders field.

/// <summary>
/// Contains data about the context in the logistic network of a node that is providing items
/// </summary>
public class ProvideData
{
    /// <summary>
    /// The count of items which are expecting a carrier to take the content.
    /// </summary>
    private Dictionary<string, int> onTheWayOrders { get; } = new();

    /// <summary>
    /// Change the count of items which are current managed by the logistic network.
    /// </summary>
    /// <param name="itemId">Id of the item.</param>
    /// <param name="amount">Delta of the amount of items. A positive value means adding, negative means removing.</param>
    public void ChangeCurrentOrders(string itemId, int amount)
    {
        // Add the request to the queue
        if (onTheWayOrders.TryGetValue(itemId, out var order))
        {
            onTheWayOrders[itemId] = order + amount;
        }
        else
        {
            onTheWayOrders.Add(itemId, amount);
        }
    }

    /// <summary>
    /// Adds the keys into the dictionary.
    /// </summary>
    [Obsolete("This is now handled automatically. Just remove this method call.")]
    public void EnsureDictionaryKeys(IEnumerable<string> keys)
    {
        foreach (var key in keys)
        {
            if (!onTheWayOrders.ContainsKey(key))
                onTheWayOrders.Add(key, 0);
        }
    }
}
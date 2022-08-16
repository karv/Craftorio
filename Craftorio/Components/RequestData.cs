using System.Collections.ObjectModel;
namespace Craftorio.Logistic;

/// <summary>
/// Contains data about the context in the logistic network of a node that is requesting items
/// </summary>
public class RequestData
{

    /// <summary>
    /// The count of items which are expecting a carrier to take the content.
    /// </summary>
    private readonly Dictionary<string, int> onTheWayOrders = new();

    /// <summary>
    /// What is the request count of items. This value is not changed by the network, so are somehow constant.
    /// </summary>
    private readonly Dictionary<string, int> requestDictionary = new();

    private readonly ReadOnlyDictionary<string, int> requestDictionaryReadOnly;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestData"/> class.
    /// </summary>
    public RequestData()
    {
        requestDictionaryReadOnly = new(requestDictionary);
    }

    /// <summary>
    /// Readonly access of the request as a dictionary.
    /// </summary>
    public ReadOnlyDictionary<string, int> AsDictionary => requestDictionaryReadOnly;

    /// <summary>
    /// Change the amount of requested items by this node in the network.
    /// </summary>
    /// <param name="itemId">Id of the item.</param>
    /// <param name="amount">Delta of the requested amount.</param>
    public void AddRequest(string itemId, int amount)
    {
        if (requestDictionary.TryGetValue(itemId, out var order))
            requestDictionary[itemId] = order + amount;
        else
            requestDictionary.Add(itemId, amount);
    }

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
    /// Updates the request dictionary of a specified item id, to a specified amount.
    /// </summary>
    public void ChangeRequestOf(string itemId, int amount)
    {
        if (requestDictionary.TryGetValue(itemId, out var order))
            requestDictionary[itemId] = amount;
        else
            requestDictionary.Add(itemId, amount);
    }

    /// <summary>
    /// Adds the keys into the dictionary.
    /// </summary>
    [Obsolete("This is now handled automatically. Just remove this method call.")]
    public void EnsureDictionaryKeys() => EnsureDictionaryKeys(requestDictionary.Keys);

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

    /// <summary>
    /// Gets the count of items of the specified ID going out from this node, which are handled by the network
    /// </summary>
    public int OrdersOf(string itemId)
    {
        return onTheWayOrders.TryGetValue(itemId, out var ret) ? ret : 0;
    }

    /// <summary>
    /// Gets a collection of items Ids that may be positive for the <see cref="OrdersOf"/> method.
    /// </summary>
    public IReadOnlyCollection<string> RequestKeys()
    {
        return onTheWayOrders.Keys;
    }

    /// <summary>
    /// Gets the amount of requests of the specified item.
    /// </summary>
    public int RequestOf(string itemId)
    {
        return onTheWayOrders.TryGetValue(itemId, out var ret) ? ret : 0;
    }
}
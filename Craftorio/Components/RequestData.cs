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
    private Dictionary<int, int> OnTheWayOrders = new Dictionary<int, int>();

    /// <summary>
    /// What is the request count of items. This value is not changed by the network, so are somehow constant.
    /// </summary>
    private Dictionary<int, int> RequestDictionary = new Dictionary<int, int>();

    public void AddRequest(int itemId, int amount)
    {
        if (RequestDictionary.TryGetValue(itemId, out var order))
        {
            RequestDictionary[itemId] = order + amount;
        }
        else
        {
            RequestDictionary.Add(itemId, amount);
        }
    }

    public ReadOnlyDictionary<int, int> AsDictionary()
    {
        return new ReadOnlyDictionary<int, int>(RequestDictionary);
    }
    public void ChangeCurrentOrders(int itemId, int amount)
    {
        // Add the request to the queue
        if (OnTheWayOrders.TryGetValue(itemId, out var order))
        {
            OnTheWayOrders[itemId] = order + amount;
        }
        else
        {
            OnTheWayOrders.Add(itemId, amount);
        }
    }

    /// <summary>
    /// Adds the keys into the dictionary.
    /// </summary>
    public void EnsureDictionaryKeys() => EnsureDictionaryKeys(RequestDictionary.Keys);

    /// <summary>
    /// Adds the keys into the dictionary.
    /// </summary>
    public void EnsureDictionaryKeys(IEnumerable<int> keys)
    {
        foreach (var key in keys)
        {
            if (!OnTheWayOrders.ContainsKey(key))
                OnTheWayOrders.Add(key, 0);
        }
    }

    public int OrdersOf(int itemId)
    {
        return OnTheWayOrders.TryGetValue(itemId, out var ret) ? ret : 0;
    }

    public IReadOnlyCollection<int> RequestKeys()
    {
        return OnTheWayOrders.Keys;
    }

    public int RequestOf(int itemId)
    {
        return OnTheWayOrders.TryGetValue(itemId, out var ret) ? ret : 0;
    }
}
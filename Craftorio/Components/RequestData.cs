namespace Craftorio.Logistic;

/// <summary>
/// Contains data about the context in the logistic network of a node that is requesting items
/// </summary>
public class RequestData
{
    /// <summary>
    /// The count of items which are expecting a carrier to take the content.
    /// </summary>
    public Dictionary<int, int> OnTheWayOrders = new Dictionary<int, int>();

    /// <summary>
    /// What is the request count of items. This value is not changed by the network, so are somehow constant.
    /// </summary>
    public Dictionary<int, int> RequestDictionary = new Dictionary<int, int>();

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
}
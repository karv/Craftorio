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
    public Dictionary<int, int> OnTheWayOrders { get; } = new Dictionary<int, int>();

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
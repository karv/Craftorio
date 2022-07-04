namespace Craftorio.Logistic;

// This should be struct, but when there is a struct for the OnTheWayOrders field.
public class ProvideData
{
    public Dictionary<int, int> OnTheWayOrders { get; } = new Dictionary<int, int>();

    public void EnsureDictionaryKeys(IEnumerable<int> keys)
    {
        foreach (var key in keys)
        {
            if (!OnTheWayOrders.ContainsKey(key))
                OnTheWayOrders.Add(key, 0);
        }
    }
}
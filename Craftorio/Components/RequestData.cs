namespace Craftorio.Logistic;

public class RequestData
{
    public Dictionary<int, int> OnTheWayOrders = new Dictionary<int, int>();
    public Dictionary<int, int> RequestDictionary = new Dictionary<int, int>();

    public void EnsureDictionaryKeys() => EnsureDictionaryKeys(RequestDictionary.Keys);
    public void EnsureDictionaryKeys(IEnumerable<int> keys)
    {
        foreach (var key in keys)
        {
            if (!OnTheWayOrders.ContainsKey(key))
                OnTheWayOrders.Add(key, 0);
        }
    }
}
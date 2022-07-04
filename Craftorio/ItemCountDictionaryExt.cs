namespace Craftorio;

public static class ItemCountDictionaryExt
{
    public static int Count(this IDictionary<int, int> dict, int index)
    {
        if (dict.TryGetValue(index, out var value))
            return value;
        return 0;
    }
}

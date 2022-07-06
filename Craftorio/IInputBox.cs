namespace Craftorio;

public interface IStoreBox : IBox
{
    public bool TryStore(ItemStack item) => TryStore(item.ItemId, item.Count);
    public bool TryStore(int itemId, int quantity);

    public bool TryStore(ReadOnlySpan<ItemStack> items);

    public int TryStoreAsMuchAsPossible(int itemId, int quantity)
    {
        var storeQt = Math.Min(quantity, FreeCapacity);
        TryStore(itemId, storeQt);
        return storeQt;
    }

}

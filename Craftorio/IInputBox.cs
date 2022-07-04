namespace Craftorio;

public interface IInputBox : IBox
{
    public bool TryStore(ItemStack item) => TryStore(item.ItemId, item.Count);
    public bool TryStore(int itemId, int quantity);

    public int TryStoreAsMuchAsPossible(int itemId, int quantity)
    {
        var storeQt = Math.Min(quantity, FreeCapacity);
        TryStore(itemId, storeQt);
        return storeQt;
    }

}

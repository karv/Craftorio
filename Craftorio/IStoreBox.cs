namespace Craftorio;

/// <summary>
/// Represents a box that can store items.
/// </summary>
public interface IStoreBox : IBox
{
    /// <summary>
    /// Try to add an item to the box.
    /// </summary>
    /// <param name="item">The stack of items to store</param>
    /// <returns>true if all the items were stores. False if no items were stored.</returns>
    public bool TryStore(ItemStack item) => TryStore(item.ItemId, item.Count);

    /// <summary>
    /// Try to add an item to the box.
    /// </summary>
    /// <returns>true if all the items were stores. False if no items were stored.</returns>
    public bool TryStore(int itemId, int quantity);

    /// <summary>
    /// Try to add an item to the box.
    /// </summary>
    /// <returns>true if all the items were stores. False if no items were stored.</returns>
    public bool TryStore(ReadOnlySpan<ItemStack> items);

    /// <summary>
    /// Try to add an item to the box.
    /// </summary>
    /// <returns>A non negative integer indicating how many items wre stored.</returns>
    public int TryStoreAsMuchAsPossible(int itemId, int quantity)
    {
        var storeQt = Math.Min(quantity, FreeCapacity);
        TryStore(itemId, storeQt);
        return storeQt;
    }
}
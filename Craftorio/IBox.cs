namespace Craftorio;

/// <summary>
/// Stores items.
/// </summary>
public interface IBox
{
    /// <summary>
    /// Gets the capacity of the box.
    /// </summary>
    public int Capacity { get; }

    /// <summary>
    /// Gets the free capacity.
    /// </summary>
    public int FreeCapacity => Capacity - UsedCapacity;

    /// <summary>
    /// Gets the used capacity.
    /// </summary>
    public int UsedCapacity { get; }

    /// <summary>
    /// Gets the amount if items of a specified type.
    /// </summary>
    public int this[string itemId] { get; }

    /// <summary>
    /// Creates a string representation of the content of the box.
    /// </summary>
    public string GetDisplayContent();
    /// <summary>
    /// Takes items from the box. This will update the content of the box.
    /// </summary>
    /// <param name="itemId">Id of the type of the item to take.</param>
    /// <param name="amount">Non negative value indicating the amount of units of the item to take.</param>
    /// <returns>An item stack representing the taken items.</returns>
    public ItemStack Take(string itemId, int amount);

    /// <summary>
    /// Try to remove the specified collection of items from the box. If the box does not contain the specified items,
    /// the method returns false and the state of the box is unchanged.
    /// </summary>
    /// <param name="items">Items to try to take</param>
    /// <returns><see langword="true"/> iff the items were taken.</returns>
    public bool TryRemoveItems(ReadOnlySpan<ItemStack> items);
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
    public bool TryStore(string itemId, int quantity);

    /// <summary>
    /// Try to add an item to the box.
    /// </summary>
    /// <returns>true if all the items were stores. False if no items were stored.</returns>
    public bool TryStore(ReadOnlySpan<ItemStack> items);

    /// <summary>
    /// Try to add an item to the box.
    /// </summary>
    /// <returns>A non negative integer indicating how many items wre stored.</returns>
    public int TryStoreAsMuchAsPossible(string itemId, int quantity)
    {
        var storeQt = Math.Min(quantity, FreeCapacity);
        TryStore(itemId, storeQt);
        return storeQt;
    }
}
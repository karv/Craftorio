namespace Craftorio;

/// <summary>
/// Represents an API interface for a box.
/// Items are stored in a dictionary, where the key is the item's id and the value is the item's quantity.
/// </summary>
public class Box : IStoreBox, ITakeableBox
{
    private const int DefaultCapacity = 10;

    /// <summary>
    /// The dictionary of items in the box. The key is the item ID, and the value is the item's quantity.
    /// </summary>
    private readonly Dictionary<int, int> items;

    private int capacity = DefaultCapacity;

    /// <summary>
    /// Initializes a new instance of the <see cref="Box"/> class.
    /// </summary>
    public Box()
    {
        items = new();
    }

    /// <summary>
    /// Gets the capacity of the box.
    /// </summary>
    public int Capacity
    {
        get => capacity;
        init
        {
            if (value < 0) throw new InvalidOperationException("Capacity must be greater than or equal to zero.");
            capacity = value;
        }
    }

    /// <summary>
    /// Gets the free capacity.
    /// </summary>
    public int FreeCapacity => Capacity - UsedCapacity;

    /// <summary>
    /// Gets the used capacity.
    /// </summary>
    public int UsedCapacity { get; private set; }

    /// <summary>
    /// Gets the amount of items of the specified type. This is equivalent to the <see cref="GetCountOf"/> method.
    /// </summary>
    public int this[int itemId] => GetCountOf(itemId);

    /// <summary>
    /// Gets a string representation of the content of the box.
    /// </summary>
    public string DisplayContent()
    {
        if (items.Count == 0) return "Empty";

        var sb = new System.Text.StringBuilder();
        foreach (var item in items)
        {
            sb.AppendLine($"{item.Value} x {item.Key}");
        }
        sb.Remove(sb.Length - 1, 1); // Remove the last \n
        return sb.ToString();
    }

    /// <summary>
    /// Gets the amount of items of the specified type.
    /// </summary>
    public int GetCountOf(int itemId)
    {
        return items.TryGetValue(itemId, out var ret) ? ret : 0;
    }

    /// <summary>
    /// Takes the specified items from this box, and updates the content of the box
    /// </summary>
    /// <param name="itemId">Type of items to take.</param>
    /// <param name="amount">Max amount of items to take.</param>
    /// <remarks>If the box doesn't contain enough items, the method will return the amount of items that could be taken.</remarks>
    /// <returns>A new stack representing the taken items.</returns>
    public ItemStack Take(int itemId, int amount)
    {
        amount = Math.Min(amount, GetCountOf(itemId));
        if (amount == 0) return default;

        // update the used capacity, amount of the item, and return the item stack.
        UsedCapacity -= amount;
        items[itemId] -= amount;
        if (items[itemId] == 0) items.Remove(itemId);
        return new ItemStack { ItemId = itemId, Count = amount };
    }

    /// <summary>
    /// Try to remove the specified collection of items from the box. If the box does not contain the specified items,
    /// the method returns false and the state of the box is unchanged.
    /// </summary>
    /// <param name="items">Items to try to take</param>
    /// <returns><see langword="true"/> iff the items were taken.</returns>
    public bool TryRemoveItems(ReadOnlySpan<ItemStack> items)
    {
        // We suppose the content of items are not repeated.

        // Check if the box contains the specified items.
        foreach (var item in items)
            if (GetCountOf(item.ItemId) < item.Count)
                return false;

        // Remove the items from the box, and return true.
        foreach (var item in items)
            RemoveUnchecked(item.ItemId, item.Count);
        return true;
    }

    /// <summary>
    /// Try to store some items into the box.
    /// </summary>
    /// <param name="itemId">Id of the item to store.</param>
    /// <param name="quantity">Positive value indicating the amount of items to store.</param>
    /// <returns><see langword="true"/> iff the items were stored.</returns>
    /// <remarks>If the box have no enough capacity, no items will be stored.</remarks>
    public bool TryStore(int itemId, int quantity)
    {
        if (quantity < 0) throw new InvalidOperationException("Quantity must be greater than or equal to zero.");
        if (quantity > FreeCapacity)
            return false;

        UncheckedStore(itemId, quantity);
        return true;
    }

    /// <summary>
    /// Try to store an array of stacks into the box.
    /// </summary>
    /// <param name="items">Items to store</param>
    /// <remarks>If the box have no enough capacity, no items will be stored.</remarks>
    /// <returns><see langword="true"/> iff the items were stored.</returns>
    public bool TryStore(ReadOnlySpan<ItemStack> items)
    {
        // Determine if there is enough space
        var storingQuantity = 0;
        foreach (var item in items)
            storingQuantity += item.Count;

        if (storingQuantity > FreeCapacity)
            return false;

        foreach (var item in items)
            UncheckedStore(item.ItemId, item.Count);

        return true;
    }

    /// <summary>
    /// Try to store as much as possible of the items into the box.
    /// </summary>
    /// <param name="itemId">Id of the item to store.</param>
    /// <param name="quantity">Positive value indicating the amount of items to store.</param>
    /// <returns><see langword="true"/> iff the items were stored.</returns>
    /// <remarks>If the box have no enough capacity, the max amount of items will be stored.</remarks>
    public int TryStoreAsMuchAsPossible(int itemId, int quantity)
    {
        var storeQt = Math.Min(quantity, FreeCapacity);
        TryStore(itemId, storeQt);
        return storeQt;
    }

    private void RemoveUnchecked(int itemId, int quantity)
    {
        if (!items.TryGetValue(itemId, out var count))
            throw new InvalidOperationException("Tried to remove an item that was not in the box.");
        items[itemId] -= quantity;
        UsedCapacity -= quantity;
    }

    private void UncheckedStore(int itemId, int quantity)
    {
        if (items.ContainsKey(itemId))
            items[itemId] += quantity;
        else
            items.Add(itemId, quantity);

        UsedCapacity += quantity;
    }
}
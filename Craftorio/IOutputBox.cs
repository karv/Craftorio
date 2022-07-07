namespace Craftorio;

/// <summary>
/// A box whose content may be taken.
/// </summary>
public interface ITakeableBox : IBox
{
    public ItemStack Take(int itemId, int amount);

    /// <summary>
    /// Try to remove the specified collection of items from the box. If the box does not contain the specified items,
    /// the method returns false and the state of the box is unchanged.
    /// </summary>
    /// <param name="items">Items to try to take</param>
    /// <returns><see langword="true"/> iff the items were taken.</returns>
    public bool TryRemoveItems(ReadOnlySpan<ItemStack> items);
}
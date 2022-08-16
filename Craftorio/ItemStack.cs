namespace Craftorio;

/// <summary>
/// Represents a stack of items of the same type.
/// </summary>
public struct ItemStack
{
    /// <summary>
    /// Count of items.
    /// </summary>
    public int Count { readonly get; set; }

    /// <summary>
    /// Type of items.
    /// </summary>
    public string ItemId { readonly get; init; }
}
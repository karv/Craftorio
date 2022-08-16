namespace Craftorio.Production;

/// <summary>
/// Determines the target (for now, of a miner).
/// </summary>

public record struct ItemTarget
{
    /// <summary>
    /// ID of the target item.
    /// </summary>
    // TODO: should be changed for an index of a recipe?
    public string ItemId;
}
namespace Craftorio.Production;

/// <summary>
/// Occurs when a miner entity completed a mining cycle, producing an item into its box.
/// </summary>
public readonly struct MiningCompleted
{
    /// <summary>
    /// Miner entity which completed the mining cycle.
    /// </summary>
    public readonly Entity Miner { get; init; }
}
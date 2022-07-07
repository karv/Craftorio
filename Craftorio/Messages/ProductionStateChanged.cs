namespace Craftorio.Production;

/// <summary>
/// Occurs when the state of a miner or assembler changes.
/// </summary>
public readonly record struct ProductionStateChanged(
    Entity ProducerEntity
);
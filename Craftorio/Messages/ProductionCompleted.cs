namespace Craftorio.Production;

/// <summary>
/// Occurs when an assembler or alike completed a recipe cycle.
/// </summary>
public readonly record struct ProductionCompleted(
    Entity ProducerEntity
);
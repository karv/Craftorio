namespace Craftorio.Production;

/// <summary>
/// Represents the production state for a producing entity, that is, an entity with the
/// <see cref="Production.TimeConsumption"/> component.
/// </summary>
[Flags]
public enum ProductionState
{
    /// <summary>
    /// The entity not initialized correctly.
    /// </summary>
    Idle = 0x0,

    /// <summary>
    /// The entity is working normally.
    /// </summary>
    Working = 0x1,

    /// <summary>
    /// The entity is manually paused. This value is flagged, so state is not lost when (un)paused.
    /// </summary>
    Paused = 0xF,

    /// <summary>
    /// The entity requires resources to work.
    /// </summary>
    WaitingForResources = 0x2,

    /// <summary>
    /// The entity is completed, but it generated resources which cannot be stored anywhere.
    /// </summary>
    OutputFull = 0x3,
}
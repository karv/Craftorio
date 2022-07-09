namespace Craftorio.Logistic;

/// <summary>
/// Occurs when an entity of carrier type is created in the world (by the logistic network).
/// </summary>
public readonly record struct CarrierCreated
{
    /// <summary>
    /// The created carrier.
    /// </summary>
    public readonly Entity Carrier { get; init; }
    public readonly Entity BaseNode { get; init; }
}
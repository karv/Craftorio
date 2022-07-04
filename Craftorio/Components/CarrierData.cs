namespace Craftorio.Logistic;

/// <summary>
/// Component unique for carriers. It stores its: content, state, and the plan of the next moves.
/// </summary>
public record struct CarrierData
{
    /// <summary>
    /// Content of the carrier.
    /// </summary>
    public ItemStack Content { get; set; }

    /// <summary>
    /// The order or plan of the carrier.
    /// </summary>
    public LogisticOrder Order { get; set; }

    /// <summary>
    /// The state of the FSM.
    /// </summary>
    public CarrierState State { get; set; }

    /// <summary>
    /// The logistic network where the carrier is.
    /// </summary>
    public LogisticNetwork Network { get; set; }
}
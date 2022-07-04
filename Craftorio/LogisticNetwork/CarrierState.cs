namespace Craftorio.Logistic;

/// <summary>
/// Possible states for a carrier entity.
/// </summary>
public enum CarrierState
{
    /// <summary>
    /// The carrier is idle. This should never happen.
    /// </summary>
    Idle,

    /// <summary>
    /// The carrier is moving to pick provided items.
    /// </summary>
    Requesting,

    /// <summary>
    /// The carrier is moving to deliver provided items.
    /// </summary>
    Delivering,

    /// <summary>
    /// The carrier is returning to base.
    /// </summary>
    Returning
}
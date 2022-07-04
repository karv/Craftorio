namespace Craftorio.Production;

/// <summary>
/// For entities which process things that takes time, this is the timer.
/// </summary>
public record struct TimeConsumption
{
    /// <summary>
    /// Progress/investment of the process.
    /// </summary>
    public int Progress;

    /// <summary>
    /// Total investment until the process is finished.
    /// </summary>
    public int Cost;

    /// <summary>
    /// How fast the progress is increased, in units per millisecond.
    /// </summary>
    public float Speed;

    /// <summary>
    /// Resets the progress to zero.
    /// </summary>
    public void Reset()
    {
        Progress = 0;
    }
}
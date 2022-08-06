namespace Craftorio.Production;

/// <summary>
/// For entities which process things that takes time, this is the timer.
/// </summary>
public record struct TimeConsumption
{
    /// <summary>
    /// The current state of consumption.
    /// </summary>
    public ProductionState ProductionState;

    /// <summary>
    /// Progress/investment of the process.
    /// </summary>
    public int Progress;

    /// <summary>
    /// Gets a value indicating whether the process is finished.
    /// </summary>
    /// <value>
    /// <c>true</c> if the process is finished; otherwise, <c>false</c>.
    /// </value>
    public bool IsCompleted => Progress >= Cost;

    public void Complete () => Progress = Cost;

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
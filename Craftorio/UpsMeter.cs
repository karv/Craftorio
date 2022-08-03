namespace Craftorio;

/// <summary>
/// Measures the mean time between update calls.
/// </summary>
public class UpsMeter
{
    /// <summary>
    /// Gets the mean time between update calls.
    /// </summary>
    public float Delay { get; private set; } = 60f;

    /// <summary>
    /// A parameter used to determine the memory of the measure. High value will make the measure
    /// forget the past values; and low value will make the measure remember the past values.
    /// </summary>
    public float MemoryWeight { get; init; } = 0.5f;

    /// <summary>
    /// Gets the mean updates per second.
    /// </summary>
    public float Ups => 1000f / Delay;

    /// <summary>
    /// Update tick. Should be called exactly once every frame.
    /// </summary>
    /// <param name="ms">Milliseconds since the last update.</param>
    public void Update(int ms)
    {
        Delay = Delay * (1 - MemoryWeight) + ms * MemoryWeight;
    }
}
namespace Craftorio;

/// <summary>
/// Stores items.
/// </summary>
public interface IBox
{

    /// <summary>
    /// Gets the capacity of the box.
    /// </summary>
    public int Capacity { get; }

    /// <summary>
    /// Gets the free capacity.
    /// </summary>
    public int FreeCapacity => Capacity - UsedCapacity;

    /// <summary>
    /// Gets the used capacity.
    /// </summary>
    public int UsedCapacity { get; }

    public int this[int itemId] { get; }
    public string DisplayContent();
}

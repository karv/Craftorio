namespace Craftorio.Logistic;

/// <summary>
/// This component is used to store the data of a constructor.
/// </summary>
public record struct ConstructorData
{
    /// <summary>
    /// The construction speed of this constructor.
    /// </summary>
    public float ConstructionSpeed;

    /// <summary>
    /// The logistic network this constructor is connected to.
    /// </summary>
    public LogisticNetwork Network;

    /// <summary>
    /// The state of this constructor. Meaning what is doing at the moment.
    /// </summary>
    public ConstructorState State;

    /// <summary>
    /// A flag enum representing the state of a constructor.
    /// </summary>
    public enum ConstructorState
    {
        /// <summary>
        /// The constructor is returning to the base specified at the moving target.
        /// </summary>
        ReturnToBase,

        /// <summary>
        /// The constructor is going to a construction site specified at the moving target.
        /// </summary>
        GoToConstructionSite
    }
}

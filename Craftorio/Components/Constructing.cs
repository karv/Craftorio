namespace Craftorio.Construction;

/// <summary>
/// Serves as a tag for entity that is currently constructing, also stores the progress and state of the construction.
/// </summary>
public record class Constructing
{
    /// <summary>
    /// The prototype of the building being constructed. This prototype must have all the components, except
    /// for the particular components, such as location.
    /// </summary>
    public EntityPrototype Prototype = EntityPrototype.Empty;

    /// <summary>
    /// A dictionary containing the resources required to construct the building.
    /// </summary>
    public Dictionary<int, int>? RequiredResources;

    /// <summary>
    /// The base time required to construct the building if the speed is 1. In milliseconds.
    /// </summary>
    public int RequiredTime;

    /// <summary>
    /// An array containing the constructors working and inside the building.
    /// </summary>
    public readonly List<Entity> ActiveConstructors = new List<Entity>(1024);
}
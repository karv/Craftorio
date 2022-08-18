namespace Craftorio;
using System.Collections.ObjectModel;
using System.Reflection;

/// <summary>
/// This class acts a prototype for an entity, containing generic information defining a class of entities.
/// </summary>
public class EntityPrototype
{
    /// <summary>
    /// Represents a prototype without any components. Use as default value being null-safe.
    /// </summary>
    public static readonly EntityPrototype Empty = new EntityPrototype();

    /// <summary>
    /// To avoid instantiating many array of length 1, we use this buffer.
    /// </summary>
    private static readonly object[] parameterBuffer = new object[1];

    /// <summary>
    /// This is the Entity.Set (T) method.
    /// </summary>
    private static readonly MethodInfo setMethod = typeof(Entity).GetMethods()[8];

    [Newtonsoft.Json.JsonProperty("Components")]
    private Dictionary<Type, Object> components = new Dictionary<Type, Object>();

    /// <summary>
    /// Adds a component to this prototype.
    /// </summary>
    /// <param name="component">Value of the component of the type <typeparamref name="T"/>.</param>
    /// <typeparam name="T">Type of component.</typeparam>
    public void AddComponent<T>(T component) where T : notnull
    {
        components.Add(typeof(T), component);
    }

    /// <summary>
    /// Adds the components of this prototype to an entity.
    /// </summary>
    /// <param name="entity">Entity where to add the components.</param>
    public void AddComponentsTo(Entity entity)
    {
        foreach (var component in components)
        {
            parameterBuffer[0] = component.Value;
            setMethod.MakeGenericMethod(component.Key).Invoke(entity, parameterBuffer);
        }
    }

    /// <summary>
    /// Creates a new entity with the components of this prototype.
    /// </summary>
    /// <param name="world">The ECS world where the entity is to be created.</param>
    /// <returns>The created entity.</returns>
    public Entity CreateEntity(World world)
    {
        var entity = world.CreateEntity();
        AddComponentsTo(entity);
        return entity;
    }

    /// <summary>
    /// Gets the component of the type <typeparamref name="T"/> from this prototype.
    /// </summary>
    public T GetComponent<T>() where T : notnull
    {
        return (T)components[typeof(T)];
    }

    /// <summary>
    /// Gets a readonly accessor to the components of this prototype, by type.
    /// </summary>
    /// <returns>A new readonly dictionary accessor whose keys are the type of the components and the value are the component of
    /// every type.</returns>
    public ReadOnlyDictionary<Type, Object> GetDictionary() => new ReadOnlyDictionary<Type, Object>(components);
}
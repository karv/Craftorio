namespace Craftorio.Construction;
using System.Collections.ObjectModel;

public class BuildingPrototype
{
    private Dictionary<Type, Object> components = new Dictionary<Type, Object>();
    public void AddComponent<T>(T component)
    {
        components.Add(typeof(T), component);
    }

    public ReadOnlyDictionary<Type, Object> GetDictionary() => new ReadOnlyDictionary<Type, Object>(components);

    private static readonly object[] parameterBuffer = new object[1];
    public void AddComponentsTo(Entity entity)
    {
            var method = entity.GetType().GetMethod("Set");
        foreach (var component in components)
        {
            // Use reflection to call the generic entity.Set method with the correct type as in the 
            // component dictionary
            var generic = method.MakeGenericMethod(component.Key);
            parameterBuffer[0] = component.Value;
            generic.Invoke(entity, parameterBuffer);
        }
    }

    public Entity CreateEntity (World world)
    {
        var entity = world.CreateEntity();
        AddComponentsTo(entity);
        return entity;
    }
}
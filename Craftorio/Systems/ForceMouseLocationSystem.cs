namespace Craftorio;
using DefaultEcs.System;

/// <summary>
/// This system forces the entities to be at the mouse location.
/// </summary>
public sealed class ForceMouseLocationSystem : AEntitySetSystem<int>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForceMouseLocationSystem"/> class.
    /// </summary>
    public ForceMouseLocationSystem(World world) :
        base(world.GetEntities()
        .With<Location>()
        .With<ForceMouseLocation>()
        .AsSet()
    )
    { }

    /// <summary>
    /// Changes the location of the (updating) entity to the mouse location plus a settable offset.
    /// </summary>
    protected override void Update(int state, in Entity entity)
    {
        ref var location = ref entity.Get<Location>();
        ref var forceMouseLocation = ref entity.Get<ForceMouseLocation>();

        // Get the camera. If too many entities of this kind are processed, consider caching the camera
        // before the loop, probably in the PreUpdate method.
        var camera = World.Get<MonoGame.Extended.OrthographicCamera>();

        location.AsVector = camera.ScreenToWorld(Microsoft.Xna.Framework.Input.Mouse.GetState().Position.ToVector2()) + forceMouseLocation.Offset;
    }
}
namespace Craftorio.Drawing;
using Microsoft.Xna.Framework.Graphics;

public sealed class SpriteDrawing : DefaultEcs.System.AEntitySetSystem<int>
{

    private readonly SpriteBatch spriteBatch;

    private MonoGame.Extended.OrthographicCamera? Camera;
    public SpriteDrawing(World world, GraphicsDevice graphicsDevice) :
    base(world.GetEntities()
        .With<Sprite>()
        .With<Location>()
        .AsSet())
    {
        spriteBatch = new SpriteBatch(graphicsDevice);
    }

    protected override void PostUpdate(int state)
    {
        spriteBatch.End();
    }

    protected override void PreUpdate(int state)
    {
        Camera = World.Get<MonoGame.Extended.OrthographicCamera>();
        spriteBatch.Begin(transformMatrix: Camera.GetViewMatrix());
    }

    protected override void Update(int state, in Entity entity)
    {
        // Do not draw if the entity is not visible.
        ref var location = ref entity.Get<Location>();
        if (!Camera.BoundingRectangle.Intersects(location.Bounds)) return;

        ref var sprite = ref entity.Get<Sprite>();
        if (sprite.Texture is null)
        {
            // If no texture is set, draw a box with the color.
            spriteBatch.FillRectangle(location.Bounds, sprite.Color);
        }
        else
        {
            // If a texture is set, draw it.
            spriteBatch.Draw(sprite.Texture, location.Bounds.ToRectangle(), sprite.Color);
        }
    }
}
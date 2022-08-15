namespace Craftorio.Drawing;
using Microsoft.Xna.Framework.Graphics;
/// <summary>
/// This system will draw all visible sprites.
/// </summary>
public sealed class SpriteDrawing : DefaultEcs.System.AEntitySetSystem<int>
{
    private readonly SpriteBatch spriteBatch;

    private MonoGame.Extended.OrthographicCamera? Camera;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteDrawing"/> class.
    /// </summary>
    public SpriteDrawing(World world, GraphicsDevice graphicsDevice) :
    base(world.GetEntities()
        .With<Sprite>()
        .With<Location>()
        .AsSet())
    {
        spriteBatch = new SpriteBatch(graphicsDevice);
    }

    /// <summary>
    /// Finishes the sprite batch.
    /// </summary>
    protected override void PostUpdate(int state)
    {
        spriteBatch.End();
    }

    /// <summary>
    /// Setup and begin the sprite batch.
    /// </summary>
    protected override void PreUpdate(int state)
    {
        Camera = World.Get<MonoGame.Extended.OrthographicCamera>();
        spriteBatch.Begin(transformMatrix: Camera.GetViewMatrix());
    }

    /// <summary>
    /// Draw, if visible, an entity.
    /// </summary>
    protected override void Update(int state, in Entity entity)
    {
        // Do not draw if the entity is not visible.
        ref var location = ref entity.Get<Location>();
        if (!Camera!.BoundingRectangle.Contains(location.AsVector)) return;

        ref var sprite = ref entity.Get<Sprite>();
        var drawingRectangle = sprite.RelativeDrawingArea.Expand(in location.AsVector);
        if (sprite.Texture is null)
        {
            // If no texture is set, draw a box with the color.
            spriteBatch.FillRectangle(drawingRectangle, sprite.Color);
        }
        else
        {
            // If a texture is set, draw it.
            spriteBatch.Draw(sprite.Texture, drawingRectangle.ToRectangle(), sprite.Color);
        }
    }
}
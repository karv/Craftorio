namespace Craftorio.Drawing;
using Microsoft.Xna.Framework.Graphics;
using DefaultEcs.System;

/// <summary>
/// This system draw texts contained in <see cref="TextSprite"/> components.
/// </summary>
public sealed class TextDrawing : AEntitySetSystem<int>
{

    private readonly SpriteBatch spriteBatch;
    private MonoGame.Extended.OrthographicCamera? camera;
    /// <summary>
    /// Initializes a new instance of the <see cref="TextDrawing"/> class.
    /// </summary>
    public TextDrawing(World world, GraphicsDevice graphicsDevice) :
        base(world.GetEntities()
            .With<TextSprite>()
            .With<Location>()
            .AsSet())
    {
        spriteBatch = new SpriteBatch(graphicsDevice);
    }

    /// <summary>
    /// Finishes the update call by ending the drawing batch.
    /// </summary>
    protected override void PostUpdate(int state)
    {
        spriteBatch.End();
    }

    /// <summary>
    /// Begins the update call, by setting the camera and the sprite batch.
    /// </summary>
    /// <param name="state"></param>
    protected override void PreUpdate(int state)
    {
        camera = World.Get<MonoGame.Extended.OrthographicCamera>();
        spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());
    }

    /// <summary>
    /// Draw the specified text entity.
    /// </summary>
    protected override void Update(int state, in Entity entity)
    {
        ref var location = ref entity.Get<Location>();
        ref var textSprite = ref entity.Get<TextSprite>();
        // Draw the background.
        spriteBatch.FillRectangle(
            location.Bounds,
            textSprite.BackgroundColor);
        // Write the text.
        spriteBatch.DrawString(textSprite.Font, textSprite.Text, location.Bounds.TopLeft, textSprite.Color);
    }
}
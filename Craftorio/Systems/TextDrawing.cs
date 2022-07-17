namespace Craftorio.Drawing;
using Microsoft.Xna.Framework.Graphics;
using DefaultEcs.System;

public sealed class TextDrawing : AEntitySetSystem<int>
{
    public TextDrawing(World world, GraphicsDevice graphicsDevice) :
        base(world.GetEntities()
            .With<TextSprite>()
            .With<Location>()
            .AsSet())
    { 
        spriteBatch = new SpriteBatch(graphicsDevice);
    }

    private readonly SpriteBatch spriteBatch;
    private MonoGame.Extended.OrthographicCamera? camera;

    protected override void PostUpdate(int state)
    {
        spriteBatch.End();
    }

    protected override void PreUpdate(int state)
    {
        camera = World.Get<MonoGame.Extended.OrthographicCamera>();
        spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());
    }

    protected override void Update(int state, in Entity entity)
    {
        ref var location = ref entity.Get<Location>();
        ref var textSprite = ref entity.Get<TextSprite>();
        spriteBatch.DrawString(textSprite.Font, textSprite.Text, location.Bounds.TopLeft, textSprite.Color);
    }
}
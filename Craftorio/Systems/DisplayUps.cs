using Microsoft.Xna.Framework.Graphics;

namespace Craftorio.Drawing.UI;

public class DisplayUps : DefaultEcs.System.ISystem<int>
{
    private readonly Microsoft.Xna.Framework.Graphics.SpriteFont font;
    private readonly Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch;
    private readonly UpsMeter upsMeter;

    public DisplayUps(Game game)
    {
        this.upsMeter = game.Services.GetService<UpsMeter>();
        font = game.Content.Load<SpriteFont>("Fonts/Default");
        spriteBatch = new SpriteBatch(game.GraphicsDevice);
    }
    public bool IsEnabled { get; set; } = true;

    public void Dispose()
    {
        spriteBatch.Dispose();
    }

    public void Update(int state)
    {
        if (!IsEnabled)
            return;

        spriteBatch.Begin();
        // Draw the ups in the top left location of the screen in white color
        spriteBatch.DrawString(font, $"UPS: {upsMeter.Ups:0.00}", new(0, 0), Color.White);
        spriteBatch.End();
    }
}
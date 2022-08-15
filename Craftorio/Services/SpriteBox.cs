namespace Craftorio;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Service class that creates information boxes as ECS entities.
/// </summary>
[Obsolete("Use the GetText method in MouseOverDisplayText component.")]
public class SpriteBox
{
    private readonly GraphicsDeviceManager graphics;
    private readonly World world;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteBox"/> class.
    /// </summary>
    /// <param name="world">The ECS world where output entities will be created.</param>
    /// <param name="graphics">The graphics device provided by Xna/MonoGame.</param>
    public SpriteBox(World world, GraphicsDeviceManager graphics)
    {
        this.world = world;
        this.graphics = graphics;
    }

    /// <summary>
    /// Gets or sets the default font for the info box. This font is used if no font is specified
    /// as an argument of the methods.
    /// </summary>
    public SpriteFont? DefaultFont { get; set; }

    /// <summary>
    /// Create a entity being a info box with the specified string and location
    /// </summary>
    public Entity CreateInfoBox(string text, Vector2 location,
    SpriteFont? font = null)
    {
        var infoBox = world.CreateEntity();
        font ??= DefaultFont ?? throw new InvalidOperationException("No default font set");

        // Create a texture with the specified text using the specified font
        var dimension = font.MeasureString(text).ToPoint();
        var texture = new Texture2D(graphics.GraphicsDevice, dimension.X, dimension.Y);
        var renderTarget = new RenderTarget2D(graphics.GraphicsDevice, dimension.X, dimension.Y);

        // Draw the text into the texture
        var spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
        graphics.GraphicsDevice.SetRenderTarget(renderTarget);
        spriteBatch.Begin();
        spriteBatch.DrawString(font, text, new(0, 0), Color.White);
        spriteBatch.End();

        // Reset the render target
        graphics.GraphicsDevice.SetRenderTarget(null);

        // Copy the texture into the entity
        infoBox.Set(new Location(location));
        infoBox.Set<Drawing.Sprite>(new Drawing.Sprite { Texture = renderTarget });
        infoBox.Set<Drawing.UI.IsInfoBox>();
        return infoBox;
    }
}
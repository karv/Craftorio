namespace Craftorio.Drawing;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Contains data on how to draw a sprite.
/// </summary>
public record struct Sprite
{
    /// <summary>
    /// The texture used to draw the sprite. If null, the sprite will not be drawn.
    /// </summary>
    public Texture2D? Texture;

    /// <summary>
    /// The color of the sprite. This is used when the texture is null.
    /// </summary>
    public Color Color;

    /// <summary>
    /// Represents the relative location of the area where the sprite will be drawn.
    /// </summary>
    public RectangleExpand RelativeDrawingArea;
}
namespace Craftorio.Drawing;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Stores information about the text on a text entity.
/// </summary>
public record struct TextSprite
{
    /// <summary>
    /// Font used to draw the text.
    /// </summary>
    public SpriteFont Font;

    /// <summary>
    /// The text to draw.
    /// </summary>
    public string Text;

    /// <summary>
    /// Color of the text.
    /// </summary>
    public Color Color;
}
namespace Craftorio.Drawing;
using Microsoft.Xna.Framework.Graphics;

public record struct TextSprite
{
    public SpriteFont Font;
    public string Text;
    public Color Color;
}
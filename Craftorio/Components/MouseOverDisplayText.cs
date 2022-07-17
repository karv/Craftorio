namespace Craftorio.Drawing.UI;

/// <summary>
/// Contains information of the tooltip hint for an entity.
/// </summary>
public struct MouseOverDisplayText
{

    public Func<Entity, string>? GetText;
    public bool IsCurrentlyDisplayed;

    public bool IsCurrentlyHovered;
    /// <summary>
    /// Display text.
    /// </summary>
    public string? Text;
}
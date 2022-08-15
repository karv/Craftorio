namespace Craftorio.Drawing.UI;

/// <summary>
/// Contains information of the tooltip hint for an entity.
/// </summary>
public struct MouseOverDisplayText
{
    /// <summary>
    /// A method used to update, when required and set, the text of the tooltip.
    /// </summary>
    public Func<Entity, string>? GetText;

    /// <summary>
    /// Determines whether (logically) the tooltip is visible.
    /// </summary>
    public bool IsCurrentlyDisplayed;

    /// <summary>
    /// Determines whether (logically) the mouse is over the entity.
    /// </summary>
    public bool IsCurrentlyHovered;

    /// <summary>
    /// Represents the relative location of the area that is sensible to the mouse, where the tooltip
    /// will be displayed.
    /// </summary>
    public RectangleExpand RelativeSensibleArea;

    /// <summary>
    /// Display text.
    /// </summary>
    public string? Text;
}
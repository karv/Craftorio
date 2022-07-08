namespace Craftorio.Production;

/// <summary>
/// A recipe as an element of the database, not as a component.
/// </summary>
public class Recipe
{
    /// <summary>
    /// The base time to complete the recipe.
    /// </summary>
    public int BaseTime { get; init; }

    /// <summary>
    /// Required inputs.
    /// </summary>
    public ItemStack[] Inputs { get; init; } = Array.Empty<ItemStack>();

    /// <summary>
    /// Required outputs.
    /// </summary>
    public ItemStack[] Outputs { get; init; } = Array.Empty<ItemStack>();

    /// <summary>
    /// Creates a new component from this recipe.
    /// </summary>
    public RecipeComponent ToComponent()
    {
        return new RecipeComponent
        {
            BaseTime = this.BaseTime,
            Inputs = this.Inputs,
            Outputs = this.Outputs
        };
    }
}
namespace Craftorio.Production;

/// <summary>
/// The recipe that is using an assembler entity.
/// </summary>
public readonly struct RecipeComponent
{
    /// <summary>
    /// The time, in milliseconds, it takes to produce the recipe is the speed is 1.
    /// </summary>
    /// <value>A non negative number.</value>
    public int BaseTime { get; init; }

    /// <summary>
    /// An array of items that are required to produce the recipe.
    /// </summary>
    public ItemStack[] Inputs { get; init; }

    /// <summary>
    /// An array of items that are produced by the recipe when the production on completed.
    /// </summary>
    public ItemStack[] Outputs { get; init; }
}
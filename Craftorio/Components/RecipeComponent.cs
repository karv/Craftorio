namespace Craftorio.Production;

public readonly struct RecipeComponent
{
    public int BaseTime { get; init; }
    public ItemStack[] Inputs { get; init; }
    public ItemStack[] Outputs { get; init; }
}
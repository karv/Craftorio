namespace Craftorio.Production;

public struct RecipeComponent
{
    public int BaseTime { get; init; }
    public ItemStack[] Inputs { get; init; }
    public ItemStack[] Outputs { get; init; }
}
namespace Craftorio.Production;

public class Recipe
{
    public int BaseTime { get; init; }
    public ItemStack[] Inputs { get; init; }
    public ItemStack[] Outputs { get; init; }

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
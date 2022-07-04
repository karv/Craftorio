namespace Craftorio;

/// <summary>
/// A box whose content may be taken.
/// </summary>
public interface IOutputBox : IBox
{
    public ItemStack Take(int itemId, int amount);
}
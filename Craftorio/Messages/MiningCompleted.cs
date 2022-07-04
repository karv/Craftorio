namespace Craftorio.Production;

public readonly struct MiningCompleted
{
    public Entity Miner { readonly get; init; }
}
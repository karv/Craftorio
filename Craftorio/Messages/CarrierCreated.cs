namespace Craftorio.Logistic;

public readonly record struct CarrierCreated
{
    public readonly Entity Carrier { get; init; }
}
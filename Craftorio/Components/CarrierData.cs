namespace Craftorio.Logistic;

public record struct CarrierData
{
    public ItemStack Content { get; set; }
    public LogisticOrder Order { get; set; }
    public CarrierState State { get; set; }
    public LogisticNetwork Network { get; set; }
}
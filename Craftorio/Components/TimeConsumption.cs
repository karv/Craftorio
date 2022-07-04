namespace Craftorio.Production;

public record struct TimeConsumption
{
    public int Progress;
    public int Cost;
    public float Speed;

    public void Reset()
    {
        Progress = 0;
    }
}

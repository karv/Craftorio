namespace Craftorio;

public class UpsMeter
{
    public float Delay { get; private set; } = 60f;
    public float Ups => 1000f / Delay;
    public float Weight { get; init; } = 0.5f;

    public void Update(int ms)
    {
        Delay = Delay * (1 - Weight) + ms * Weight;
    }
}
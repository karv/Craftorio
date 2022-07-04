namespace Craftorio;
public record struct Location
{
    public Vector2 AsVector;

    public readonly float X => AsVector.X;
    public readonly float Y => AsVector.Y;
}


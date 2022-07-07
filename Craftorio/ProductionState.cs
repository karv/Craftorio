namespace Craftorio.Production;

[Flags]
public enum ProductionState
{
    Idle = 0x0,
    Working = 0x1,
    Paused = 0xF,
    WaitingForResources = 0x2,
    OutputFull = 0x3,
}
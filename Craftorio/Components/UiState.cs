namespace Craftorio;

/// <summary>
/// A singleton component to store the state of the UI.
/// </summary>
public record struct UiState
{
    /// <summary>
    /// If not null, the game is waiting for the player to select a location to build a building.
    /// </summary>
    public EntityPrototype? SelectedEntityPrototypeForConstruction;
}
namespace Craftorio.Construction;
using System.Collections.Generic;
using CE.MG.Input;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// This reactive system will construct buildings, when a building prototype is active.
/// It is triggered by the left mouse click.
/// </summary>
public class BuildingPrototypeReactive
{
    private readonly EntitySet buildingSet;
    private readonly EntityFactory factory;
    private readonly CE.Comm.ISubscription mouseSubscription;

    private readonly World world;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildingPrototypeReactive"/> class.
    /// </summary>
    /// <param name="world">The WCS world where buildings are to be constructed.</param>
    /// <param name="inputsPublisher">The manager of mouse input.</param>
    /// <param name="factory">Factory of prototypes. Used to create Construction prototypes.</param>
    public BuildingPrototypeReactive(World world, CE.Comm.EventPublisher inputsPublisher, EntityFactory factory)
    {
        this.world = world;
        // Create a subscription to the mouse listener
        mouseSubscription = inputsPublisher.Subscribe<MouseEventArgs>(HandleMouse);
        buildingSet = world.GetEntities().With<ForceMouseLocation>().AsSet();
        this.factory = factory;
    }

    /// <summary>
    /// Gets a value indicating whether this system is enabled.
    /// </summary>
    public bool IsEnabled
    {
        get => mouseSubscription.State == CE.Comm.SubscriptionState.Active;
        set
        {
            if (value)
                mouseSubscription.Continue();
            else
                mouseSubscription.Pause();
        }
    }

    /// <summary>
    /// Dispose this system.
    /// Once disposed never enable this instance again.
    /// </summary>
    public void Dispose()
    {
        // Dispose of the subscription
        mouseSubscription.Dispose();
    }

    private void HandleMouse(MouseEventArgs args)
    {
        if (args.Event != MouseEventArgs.EventType.TouchPress) return;
        // Verify that the world is in construction mode, by checking if the UiState has a building prototype
        var prototype = world.Get<UiState>().SelectedEntityPrototypeForConstruction;
        if (prototype is null) return;

        // As we are in construction mode, the list in the buildingSet must not be empty; we trust that
        // Get its first element
        var cursorEntity = buildingSet.GetEntities()[0];

        // For now, the required resources
        var requiredResources = new Dictionary<string, int> { { "1", 1 }, { "2", 1 } };

        // Create a new construction entity
        var constructionEntity = factory.CreateConstruction(
            cursorEntity.Get<Location>().AsVector,
            prototype,
            requiredResources,
            5000);

        // If the shift key is not pressed, remove the building state
        if (!Keyboard.GetState().IsKeyDown(Keys.LeftShift))
        {
            // Remove the building state
            world.Get<UiState>().SelectedEntityPrototypeForConstruction = null;

            // Remove the cursor entity
            cursorEntity.Dispose();
        }
    }
}
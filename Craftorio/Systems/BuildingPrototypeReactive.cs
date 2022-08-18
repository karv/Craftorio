namespace Craftorio.Construction;
using System.Collections.Generic;
using DefaultEcs.System;
using Microsoft.Xna.Framework.Input;
using CE.MG.Input;

public class BuildingPrototypeReactive
{
    private readonly EntitySet buildingSet;
    private readonly EntityFactory factory;
    private readonly CE.Comm.ISubscription mouseSubscription;
    public BuildingPrototypeReactive(World world, CE.Comm.EventPublisher inputsPublisher, EntityFactory factory)
    {
        this.world = world;
        // Create a subscription to the mouse listener
        mouseSubscription = inputsPublisher.Subscribe<MouseEventArgs>(HandleMouse);
        buildingSet = world.GetEntities().With<ForceMouseLocation>().AsSet();
        this.factory = factory;
    }

    public bool IsEnabled { get; set; } = true;
    public World world { get; }

    public void Dispose()
    {
        // Dispose of the subscription
        mouseSubscription.Dispose();
    }

    public void Update(int state)
    {
        throw new NotImplementedException();
    }

    private void HandleMouse(MouseEventArgs args)
    {
        if (args.Event == MouseEventArgs.EventType.TouchPress)
        {
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
}
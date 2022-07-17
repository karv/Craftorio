namespace Craftorio.Drawing.UI;
using System;
using DefaultEcs.System;
using Microsoft.Xna.Framework.Graphics;

public sealed class DisplayTextGenerator : AEntitySetSystem<int>
{
    private const int MouseOverDisplayTextTime = 1000; // 1 second.

    private readonly EntitySet toolTipEntities;
    private bool IsClearingInfoBoxes = false;
    private OrthographicCamera? camera;
    private int mouseOverTimer;
    public DisplayTextGenerator(World world) :
        base(world.GetEntities()
            .With<MouseOverDisplayText>()
            .With<Location>()
            .AsSet())
    {
        toolTipEntities = world.GetEntities()
            .With<IsInfoBox>()
            .AsSet();
    }
    public SpriteFont Font { get; set; }

    protected override void PostUpdate(int state)
    {
        if (IsClearingInfoBoxes && toolTipEntities.Count > 0)
        {
            var tooltipEntity = toolTipEntities.GetEntities()[0];
            tooltipEntity.Dispose();
        }
    }

    protected override void PreUpdate(int state)
    {
        // Get the camera from the world
        camera = World.Get<OrthographicCamera>();

        IsClearingInfoBoxes = false;
    }

    protected override void Update(int state, ReadOnlySpan<Entity> entities)
    {
        // Only update the first entity.
        var entity = entities[0];

        // Get the location and display state of this entity
        ref var location = ref entity.Get<Location>();
        ref var displayState = ref entity.Get<MouseOverDisplayText>();

        // if the mouse is over this entity:
        var mouseScreenLocation = Microsoft.Xna.Framework.Input.Mouse.GetState().Position.ToVector2();
        var mouseWorldLocation = camera!.ScreenToWorld(mouseScreenLocation);
        displayState.IsCurrentlyHovered = location.Bounds.Contains(mouseWorldLocation);
        if (displayState.IsCurrentlyHovered)
        {
            // If the mouse is not currently displayed, update the timer.
            if (!displayState.IsCurrentlyDisplayed)
            {
                if (mouseOverTimer > MouseOverDisplayTextTime)
                {
                    displayState.IsCurrentlyDisplayed = true;
                    displayState.Text = displayState.GetText?.Invoke(entity) ?? "";
                    // Create the entity
                    var hintEntity = World.CreateEntity();
                    hintEntity.Set(new Location(location.Bounds));
                    hintEntity.Set(new TextSprite
                    {
                        Font = Font,
                        Text = displayState.Text,
                        Color = Color.White
                    });
                    hintEntity.Set<IsInfoBox>();
                }
                else
                    mouseOverTimer += state;
            }
        }
        else  // Mouse is not over this entity.
        {
            // Reset the timer...
            mouseOverTimer = 0;

            // ... and the entity is marked as being displayed, flag to removal and update the state.
            if (displayState.IsCurrentlyDisplayed)
            {
                IsClearingInfoBoxes = true;
                displayState.IsCurrentlyDisplayed = false;
            }
        }
    }
}
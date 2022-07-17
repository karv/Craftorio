namespace Craftorio.Drawing.UI;
using System;
using DefaultEcs.System;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Produce tooltip entities from mouseover events.
/// </summary>
public sealed class DisplayTextGenerator : AEntitySetSystem<int>
{
    /// <summary>
    /// Time until the tooltip is shown.
    /// </summary>
    private const int MouseOverDisplayTextTime = 1000; // 1 second.
    /// <summary>
    /// Collection of tooltip entities.
    /// </summary>
    private readonly EntitySet toolTipEntities;

    /// <summary>
    /// When this field is set to true, all (at most only one could exist.) tooltip entities will be removed.
    /// </summary>
    private bool IsClearingInfoBoxes = false;

    private OrthographicCamera? camera;

    /// <summary>
    /// Measure the time the mouse have been over an entity.
    /// </summary>
    private int mouseOverTimer;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayTextGenerator"/> class.
    /// </summary>
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

    /// <summary>
    /// Font used to produce the tooltip entities.
    /// </summary>
    /// <value>A value must be set, otherwise an exception could be thrown.</value>
    public SpriteFont Font { get; init; }

    /// <summary>
    /// After the update call, if the <see cref="IsClearingInfoBoxes"/> field is set to true, all tooltip entities will be removed.
    /// </summary>
    protected override void PostUpdate(int state)
    {
        if (IsClearingInfoBoxes && toolTipEntities.Count > 0)
        {
            var tooltipEntity = toolTipEntities.GetEntities()[0];
            tooltipEntity.Dispose();
        }
    }

    /// <summary>
    /// Updates the camera.
    /// </summary>
    /// <param name="state"></param>
    protected override void PreUpdate(int state)
    {
        // Get the camera from the world
        camera = World.Get<OrthographicCamera>();

        IsClearingInfoBoxes = false;
    }

    protected void Update(int state, in Entity entity, out bool entityCatched)
    {
        // Get the location and display state of this entity
        ref var location = ref entity.Get<Location>();
        ref var displayState = ref entity.Get<MouseOverDisplayText>();

        // if the mouse is over this entity:
        var mouseScreenLocation = Microsoft.Xna.Framework.Input.Mouse.GetState().Position.ToVector2();
        var mouseWorldLocation = camera!.ScreenToWorld(mouseScreenLocation);
        entityCatched = displayState.IsCurrentlyHovered = location.Bounds.Contains(mouseWorldLocation);
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
    protected override void Update(int state, ReadOnlySpan<Entity> entities)
    {
        // Only update the first entity.
        foreach (ref readonly var entity in entities)
        {
            Update(state, entity, out var entityCatched);

            // If an entity got the mouse over state, we can stop the loop.
            if (entityCatched)
                break;
        }
    }
}
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
    public SpriteFont? Font { get; set; }

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

    /// <summary>
    /// Updates the entities in the system.
    /// </summary>
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

    /// <summary>
    /// Updates the entities, to determine whether create or remove tooltip entities.
    /// </summary>
    /// <param name="state">Elapsed milliseconds.</param>
    /// <param name="entity">Updating entity.</param>
    /// <param name="entityCatched">outs <see langword="true"/>, iff the current entity catched the mouse</param>
    private void Update(int state, in Entity entity, out bool entityCatched)
    {
        // Get the location and display state of this entity
        ref var location = ref entity.Get<Location>();
        ref var displayState = ref entity.Get<MouseOverDisplayText>();

        // if the mouse is over this entity:
        var mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
        var mouseScreenLocation = mouseState.Position.ToVector2();
        var mouseWorldLocation = camera!.ScreenToWorld(mouseScreenLocation);
        entityCatched = displayState.IsCurrentlyHovered = location.Bounds.Contains(mouseWorldLocation);

        // This should be refactored
        if (displayState.IsCurrentlyHovered)
        {
            // If the mouse is not currently displayed, update the timer.
            if (!displayState.IsCurrentlyDisplayed)
            {
                if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl))
                {
                    displayState.IsCurrentlyDisplayed = true;
                    displayState.Text = displayState.GetText?.Invoke(entity) ?? "";
                    // Create the entity
                    var hintEntity = World.CreateEntity();

                    // Use the measure of the font to get the smallest rectangle that fits the text.
                    var textSize = Font!.MeasureString(displayState.Text);
                    var textBounds = new RectangleF(
                        location.Bounds.TopLeft,
                        textSize);
                    hintEntity.Set(new Location(
                        new RectangleF(
                            textBounds.TopLeft,
                            textBounds.Size))
                    );
                    hintEntity.Set(new TextSprite
                    {
                        Font = Font!,  // Se assume the font is already set, otherwise its clear that the game must break.
                        Text = displayState.Text,
                        Color = Color.White,
                        BackgroundColor = Color.Black * 0.67f,
                    });
                    hintEntity.Set<IsInfoBox>();
                }
            }
        }
        else  // Mouse is not over this entity.
        {
            // ... and the entity is marked as being displayed, flag to removal and update the state.
            if (displayState.IsCurrentlyDisplayed)
            {
                IsClearingInfoBoxes = true;
                displayState.IsCurrentlyDisplayed = false;
            }
        }
    }
}
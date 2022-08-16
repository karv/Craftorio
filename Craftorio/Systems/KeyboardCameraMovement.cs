namespace Craftorio;
using DefaultEcs.System;

/// <summary>
/// Controls the camera with the keyboard.
/// </summary>
public sealed class KeyboardCameraMovement : ISystem<int>
{
    private readonly World world;

    const float cameraMovementSpeed = 0.3f;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardCameraMovement"/> class.
    /// </summary>
    public KeyboardCameraMovement(World world)
    {
        this.world = world;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="KeyboardCameraMovement"/> is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Reads the keyboard state and updates the position of the camera.
    /// </summary>
    public void Update(int state)
    {
        if (!IsEnabled) return;

        // Get the keyboard status and determine the movement vector.
        var keyboardState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
        var movement = new Microsoft.Xna.Framework.Vector2(
            (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D) ? 1 : 0) +
                (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A) ? -1 : 0),
            (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S) ? 1 : 0) +
                (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W) ? -1 : 0)
        );
        if (movement.LengthSquared() == 0) return;

        movement.Normalize();
        movement *= cameraMovementSpeed * state;

        var camera = world.Get<MonoGame.Extended.OrthographicCamera>();
        camera.Position += movement;
    }

    void IDisposable.Dispose() { }
}
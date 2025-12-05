/// <file>
/// <summary>
/// KeyboardInput.cs - Keyboard input implementation for desktop devices
/// </summary>
/// </file>

using Godot;
using System;

/// <summary>
/// Keyboard input implementation of IInput interface.
/// </summary>
/// <remarks>
/// Uses physical keyboard keys (WASD) for movement.
/// Maps specific keys for dash (Shift) and shoot (J) actions.
/// </remarks>

public partial class KeyboardInput : IInput
{
    /// <summary>
    /// Gets movement direction based on WASD keys.
    /// </summary>
    /// <returns>
    /// Vector2 where Y is -1 for W, +1 for S; X is -1 for A, +1 for D.
    /// Returns Vector2.Zero if no keys are pressed.
    /// </returns>

    public Vector2 GetMovement()
    {
        Vector2 m = Vector2.Zero;
        if (Input.IsKeyPressed(Key.W)) m.Y -= 1;
        if (Input.IsKeyPressed(Key.S)) m.Y += 1;
        if (Input.IsKeyPressed(Key.A)) m.X -= 1;
        if (Input.IsKeyPressed(Key.D)) m.X += 1;
        return m;
    }

    /// <summary>
    /// Checks if dash action is triggered via keyboard.
    /// </summary>
    /// <returns>True if Shift key is pressed; otherwise false.</returns>

    public bool ToDash()
    {
        return Input.IsKeyPressed(Key.Shift);
    }

    /// <summary>
    /// Checks if shoot action is triggered via keyboard.
    /// </summary>
    /// <returns>True if J key is pressed; otherwise false.</returns>
    /// <remarks>
    /// Currently mapped to the 'J' key for quick testing.
    /// </remarks>
    public bool ToShoot()
    {
        return Input.IsKeyPressed(Key.J);
    }
}
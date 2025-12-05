/// <file>
/// <summary>
/// IInput.cs - Interface definition for game input strategies
/// </summary>
/// </file>

using Godot;
using System;

/// <summary>
/// Interface for input handling strategies.
/// </summary>
/// <remarks>
/// Defines the contract that all input methods (Keyboard, Joystick, AI) must implement.
/// </remarks>
public interface IInput
{
    /// <summary>
    /// Gets movement direction vector.
    /// </summary>
    /// <returns>A Vector2 representing the desired movement direction.</returns>
    Vector2 GetMovement();

    /// <summary>
    /// Checks if shoot action is triggered.
    /// </summary>
    /// <returns>True if the shoot input is active; otherwise false.</returns>
    bool ToShoot();

    /// <summary>
    /// Checks if dash action is triggered.
    /// </summary>
    /// <returns>True if the dash input is active; otherwise false.</returns>
    bool ToDash();
}
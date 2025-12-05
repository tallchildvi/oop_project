/// <file>
/// <summary>
/// LevelUI.cs - Manages the user interface elements specific to the active game level.
/// </summary>
/// <remarks>
/// This script handles the initialization and control of various UI components
/// that are displayed during gameplay, such as the joystick control and health/score displays (if added).
/// It inherits from <see cref="Node"/> as it acts as a container/controller for UI elements.
/// </remarks>
/// </file>
using Godot;
using System;

/// <summary>
/// A controller class for the heads-up display (HUD) and user interface elements
/// that are active during a level of gameplay.
/// </summary>
public partial class LevelUI : Node
{
    /// <summary>
    /// Reference to the joystick control component used for player movement input.
    /// Assumes <c>JoyStick</c> is a custom control node.
    /// </summary>
    private JoyStick _joyStick;

    /// <summary>
    /// Method to explicitly show or activate the level UI elements.
    /// Currently only logs a message to the console.
    /// </summary>
    public void Show(){
        GD.Print("show ui");
    }
}

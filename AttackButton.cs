/// <file>
/// <summary>
/// AttackButton.cs - Maps physical button presses to a virtual Godot Input action ("shoot").
/// </summary>
/// <remarks>
/// This script extends <see cref="TextureButton"/> and is primarily used for mobile or
/// touch-screen input where pressing the UI button simulates pressing and releasing a
/// keyboard/controller action named "shoot" in the Godot Input Map.
/// </remarks>
/// </file>
using Godot;
using System;

/// <summary>
/// A specialized UI button that translates physical button interaction (press/release)
/// into corresponding virtual actions on the Godot Input Map.
/// </summary>
public partial class AttackButton : TextureButton
{
    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// Connects the <see cref="Button.ButtonDown"/> and <see cref="Button.ButtonUp"/> signals
    /// to the respective handler methods.
    /// </summary>
    public override void _Ready()
    {
        this.ButtonDown += OnButtonDown;
        this.ButtonUp += OnButtonUp;
    }

    /// <summary>
    /// Handler for the <see cref="Button.ButtonDown"/> signal.
    /// This method simulates a **press** of the input action named "shoot".
    /// </summary>
    private void OnButtonDown()
    {
        Input.ActionPress("shoot");
    }

    /// <summary>
    /// Handler for the <see cref="Button.ButtonUp"/> signal.
    /// This method simulates a **release** of the input action named "shoot".
    /// </summary>
    private void OnButtonUp()
    {
        Input.ActionRelease("shoot");
    }
}

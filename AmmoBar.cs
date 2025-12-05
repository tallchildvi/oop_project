/// <file>
/// <summary>
/// AmmoBar.cs - UI component for displaying player ammunition
/// </summary>
/// </file>

using Godot;
using System;

/// <summary>
/// Manages the visual representation of the player's ammunition supply.
/// </summary>
/// <remarks>
/// Inherits from <see cref="TextureProgressBar"/>.
/// Acts as a passive listener using the <see cref="EventManager"/> to update UI 
/// whenever "PLAYER_AMMO_CHANGED" is triggered.
/// </remarks>
public partial class AmmoBar : TextureProgressBar
{
    /// <summary>
    /// Initializes the bar limits and subscribes to ammo events.
    /// </summary>
    public override void _Ready()
    {
        MinValue = 0;
        EventManager.Subscribe("PLAYER_AMMO_CHANGED", OnAmmoChanged);
        
        GD.Print("[AmmoBar] Subscribed to PLAYER_AMMO_CHANGED");
    }
    
    /// <summary>
    /// Callback triggered when player ammo count changes.
    /// </summary>
    /// <param name="data">Expected to be an instance of <see cref="AmmoData"/> containing current and max ammo.</param>
    /// <remarks>
    /// Updates <see cref="Range.MaxValue"/> and <see cref="Range.Value"/>.
    /// Clamps the value to ensure valid UI rendering.
    /// Logs an error if the received data is not of type <see cref="AmmoData"/>.
    /// </remarks>
    private void OnAmmoChanged(object data)
    {
        if (data is AmmoData ammoData)
        {
            MaxValue = ammoData.MaxAmmo;
            Value = Mathf.Clamp(ammoData.CurrentAmmo, MinValue, MaxValue);
            
            GD.Print($"[AmmoBar] Updated: {Value}/{MaxValue}");
        }
        else
        {
            GD.PrintErr("[AmmoBar] Received invalid ammo data");
        }
    }
    
    /// <summary>
    /// Cleans up event subscriptions when the node is removed from the scene tree.
    /// </summary>
    /// <remarks>
    /// Essential to prevent memory leaks or calls to disposed objects.
    /// </remarks>
    public override void _ExitTree()
    {
        EventManager.Unsubscribe("PLAYER_AMMO_CHANGED", OnAmmoChanged);
        GD.Print("[AmmoBar] Unsubscribed from PLAYER_AMMO_CHANGED");
    }
}
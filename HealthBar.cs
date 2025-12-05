/// <file>
/// <summary>
/// HealthBar.cs - UI component for displaying player health
/// </summary>
/// </file>

using Godot;
using System;

/// <summary>
/// Manages the visual representation of the player's health.
/// </summary>
/// <remarks>
/// Inherits from <see cref="TextureProgressBar"/>.
/// Acts as a passive listener using the <see cref="EventManager"/> to update UI 
/// whenever "PLAYER_HEALTH_CHANGED" is triggered.
/// </remarks>
public partial class HealthBar : TextureProgressBar
{
	/// <summary>
	/// Initializes the bar and subscribes to health events.
	/// </summary>
	public override void _Ready()
	{
		MinValue = 0;
		EventManager.Subscribe("PLAYER_HEALTH_CHANGED", OnHealthChanged);
		
		GD.Print("[HealthBar] Subscribed to PLAYER_HEALTH_CHANGED");
	}

	/// <summary>
	/// Callback triggered when player health changes.
	/// </summary>
	/// <param name="data">Expected to be an instance of <see cref="HealthData"/> containing current and max health.</param>
	/// <remarks>
	/// Updates <see cref="Range.MaxValue"/> and <see cref="Range.Value"/>.
	/// Clamps the value to ensure valid UI rendering.
	/// Logs an error if the received data is not of type <see cref="HealthData"/>.
	/// </remarks>
	private void OnHealthChanged(object data)
	{
		if (data is HealthData healthData)
		{
			MaxValue = healthData.MaxHealth;
			Value = Mathf.Clamp(healthData.CurrentHealth, MinValue, MaxValue);
			
			GD.Print($"[HealthBar] Updated: {Value}/{MaxValue}");
		}
		else
		{
			GD.PrintErr("[HealthBar] Received invalid health data");
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
		EventManager.Unsubscribe("PLAYER_HEALTH_CHANGED", OnHealthChanged);
		GD.Print("[HealthBar] Unsubscribed from PLAYER_HEALTH_CHANGED");
	}
}

using Godot;
using System;

public partial class HealthBar : TextureProgressBar
{
	public override void _Ready()
	{
		MinValue = 0;
		EventManager.Subscribe("PLAYER_HEALTH_CHANGED", OnHealthChanged);
		
		GD.Print("[HealthBar] Subscribed to PLAYER_HEALTH_CHANGED");
	}

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

	public override void _ExitTree()
	{
		EventManager.Unsubscribe("PLAYER_HEALTH_CHANGED", OnHealthChanged);
		GD.Print("[HealthBar] Unsubscribed from PLAYER_HEALTH_CHANGED");
	}
}

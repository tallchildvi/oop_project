using Godot;
using System;

public partial class AmmoBar : TextureProgressBar
{
	public override void _Ready()
	{
		MinValue = 0;
		EventManager.Subscribe("PLAYER_AMMO_CHANGED", OnAmmoChanged);
		
		GD.Print("[AmmoBar] Subscribed to PLAYER_AMMO_CHANGED");
	}
	
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
	
	public override void _ExitTree()
	{
		EventManager.Unsubscribe("PLAYER_AMMO_CHANGED", OnAmmoChanged);
		GD.Print("[AmmoBar] Unsubscribed from PLAYER_AMMO_CHANGED");
	}
}

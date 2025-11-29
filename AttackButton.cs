using Godot;
using System;

public partial class AttackButton : TextureButton
{
	public override void _Ready()
	{
		this.ButtonDown += OnButtonDown;
		this.ButtonUp += OnButtonUp;
	}

	private void OnButtonDown()
	{
		Input.ActionPress("shoot");
	}

	private void OnButtonUp()
	{
		Input.ActionRelease("shoot");
	}
} 

using Godot;
using System;

public partial class Scooter : Area2D
{
	public CharacterManager manager;
	private MainCharacter character;
	private float originalSpeed;
	public float hp = 4f;

	public override void _Ready()
	{
		GD.Print("Scooter ready!");

		Connect("area_entered", new Callable(this, nameof(OnAreaEntered)));
		character = GetParent() as MainCharacter;
		if (character == null)
		{
			GD.PrintErr("Scooter is not a child of a MainCharacter!");
			return;
		}
		originalSpeed = character.speed;
		character.SetSpeed(originalSpeed * 2f);
	}

	private void OnAreaEntered(Area2D area)
	{
		if (character == null)
			return;
		if (area is ReloadBox)
		{
			hp -= 1f;
			GD.Print($"Scooter took damage! HP left: {hp}");

			if (hp <= 0f)
			{
				GD.Print("Scooter destroyed! Reverting speed.");
				character.speed = originalSpeed;
				QueueFree();
			}
		}

		if (area is ReloadBox && manager != null)
		{
			GD.Print("Scooter entered ReloadBox!");
			manager.Reload(5);
		}
	}
}

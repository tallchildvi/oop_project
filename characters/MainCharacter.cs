using Godot;
using System;

public partial class MainCharacter : Sprite2D
{
	private float speed = 200f;
	private Vector2 originalWeaponScale;

	public override void _Ready()
	{
		// Save original weapon scale once
		Sprite2D weapon = GetNode<Sprite2D>("weapon");
		originalWeaponScale = weapon.Scale;
	}

	public override void _Process(double delta)
	{
		Sprite2D weapon = GetNode<Sprite2D>("weapon");
		Vector2 movement = Vector2.Zero;

		if (Input.IsKeyPressed(Key.W))
			movement.Y -= 1;
		if (Input.IsKeyPressed(Key.S))
			movement.Y += 1;
		if (Input.IsKeyPressed(Key.A))
			movement.X -= 1;
		if (Input.IsKeyPressed(Key.D))
			movement.X += 1;

		if (movement != Vector2.Zero)
		{
			movement = movement.Normalized();
			Position += movement * speed * (float)delta;

			float angle = Mathf.Atan2(movement.Y, movement.X);

			// ✅ Flip only when X direction changes - Doesn't work...
			if (movement.X < 0)
			{
				weapon.Scale = new Vector2(-Mathf.Abs(originalWeaponScale.X), originalWeaponScale.Y);
				angle += Mathf.Pi; // rotate 180° if needed
			}
			else if (movement.X > 0)
			{
				weapon.Scale = new Vector2(Mathf.Abs(originalWeaponScale.X), originalWeaponScale.Y);
			}

			weapon.RotationDegrees = Mathf.RadToDeg(angle);
		}
	}
}

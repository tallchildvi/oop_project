using Godot;
using System;

public partial class KeyboardInput : IInput
{
	public Vector2 GetMovement()
	{
		Vector2 m = Vector2.Zero;
		if (Input.IsKeyPressed(Key.W)) m.Y -= 1;
		if (Input.IsKeyPressed(Key.S)) m.Y += 1;
		if (Input.IsKeyPressed(Key.A)) m.X -= 1;
		if (Input.IsKeyPressed(Key.D)) m.X += 1;
		return m;
	}

	public bool ToDash()
	{
		return Input.IsKeyPressed(Key.Shift);
	}

	public bool ToShoot()
	{
		return Input.IsKeyPressed(Key.J);
	}
}

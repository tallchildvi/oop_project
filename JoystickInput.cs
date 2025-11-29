using Godot;
using System;

public partial class JoystickInput : IInput
{
	private JoyStick joystick;

	public JoystickInput(JoyStick js)
	{
		joystick = js;
	}

	public Vector2 GetMovement()
	{
		return joystick.GetDirection();
	}

	public bool ToDash()
	{
		//return Input.IsActionPressed("dash"); 
	}

	public bool WantsToShoot()
	{
		//return Input.IsActionPressed("shoot");
	}
}

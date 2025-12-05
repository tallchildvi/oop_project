/// <file>
/// <summary>
/// JoystickInput.cs - Virtual joystick input implementation for mobile devices
/// </summary>
/// </file>

using Godot;
using System;

/// <summary>
/// Virtual joystick input implementation of IInput interface.
/// </summary>
/// <remarks>
/// Uses a JoyStick UI component for movement on mobile devices.
/// Combines joystick for movement with input actions for dash and shoot.
/// </remarks>
public partial class JoystickInput : IInput
{
	/// <summary>Reference to the virtual joystick UI component.</summary>
	private JoyStick joystick;

	/// <summary>
	/// Initializes joystick input with a JoyStick component.
	/// </summary>
	/// <param name="js">The JoyStick UI component to use for movement input.</param>
	public JoystickInput(JoyStick js)
	{
		joystick = js;
	}

	/// <summary>
	/// Gets movement direction from the virtual joystick.
	/// </summary>
	/// <returns>Direction vector from joystick (typically normalized).</returns>
	public Vector2 GetMovement()
	{
		return joystick.GetDirection();
	}

	/// <summary>
	/// Checks if dash action is triggered.
	/// </summary>
	/// <returns>True if Shift key is pressed; otherwise false.</returns>
	/// <remarks>
	/// Currently uses Shift key, but can be changed to use input actions.
	/// </remarks>
	public bool ToDash()
	{
		return Input.IsKeyPressed(Key.Shift);
		//return Input.IsActionPressed("dash"); 
	}

	/// <summary>
	/// Checks if shoot action is triggered.
	/// </summary>
	/// <returns>True if "shoot" action is pressed; otherwise false.</returns>
	/// <remarks>
	/// Uses Godot's input action system for flexible button mapping.
	/// </remarks>
	public bool ToShoot()
	{
		return Input.IsActionPressed("shoot");
		//return Input.IsActionPressed("shoot");
	}
}
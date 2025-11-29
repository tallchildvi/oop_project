using Godot;
using System;

public interface IInput
{
	Vector2 GetMovement();
	bool ToShoot();
	bool ToDash();
}

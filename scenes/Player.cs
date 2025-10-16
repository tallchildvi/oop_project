using Godot;
using System;

public partial class Player : Node2D
{
	[Export] public float Speed = 200f;

	private Vector2 _inputDir;

	public override void _PhysicsProcess(double delta)
	{
		//_inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		//Velocity = _inputDir * Speed;
		//MoveAndSlide();
	}

	public override void _Ready()
	{
		GD.Print("[Player] Ready!");
	}
	public void EnableControl(bool conrol){
		//
	}
}

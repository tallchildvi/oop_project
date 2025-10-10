using Godot;
using System;

public partial class StandartBullet : Sprite2D
{
	private Vector2 movement;
	private float speed = 400f;
	private float spinSpeed = 720f;
	
	public void Init(Vector2 direction, bool facingRight)
	{
		movement = direction.Normalized();
		if (movement == Vector2.Zero){
			if(facingRight) movement.X += 1;
			else movement.X -= 1;
		}
	}
	
	public override void _Process(double delta)
	{
		Position += movement * speed * (float)delta;
		RotationDegrees += spinSpeed * (float)delta;
	}
}

using Godot;
using System;

public partial class StandartBullet : BulletInheritance
{
	public override void _Ready()
	{
		base._Ready();
		selfGroup = "player_bullet";
		oppositeGroup = "enemy";
		speed = 1000f;
	}

	public override void Init(Vector2 direction, bool facingRight)
	{
		base.Init(direction, facingRight);
		AddToGroup("player_bullet");
	}
}

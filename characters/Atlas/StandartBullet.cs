using Godot;
<<<<<<< HEAD
using System;
=======
>>>>>>> main

public partial class StandartBullet : BulletInheritance
{
	public override void _Ready()
	{
<<<<<<< HEAD
		base._Ready();
		selfGroup = "player_bullet";
		oppositeGroup = "enemy";
		speed = 2000f;
	}

	public override void Init(Vector2 direction, bool facingRight)
	{
		base.Init(direction, facingRight);
		AddToGroup("player_bullet");
=======
		selfGroup = "player_bullet";
		oppositeGroup = "enemy_bullet";
		speed = 400f;
		base._Ready();
>>>>>>> main
	}
}

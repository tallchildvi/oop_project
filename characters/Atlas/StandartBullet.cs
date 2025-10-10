using Godot;

public partial class StandartBullet : BulletInheritance
{
	public override void _Ready()
	{
		selfGroup = "player_bullet";
		oppositeGroup = "enemy_bullet";
		speed = 400f;
		base._Ready();
	}
}

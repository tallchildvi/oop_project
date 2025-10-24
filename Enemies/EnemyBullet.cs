using Godot;

public partial class EnemyBullet : BulletInheritance
{
	public override void _Ready()
	{
		selfGroup = "enemy_bullet";
		oppositeGroup = "player_bullet";
		speed = 500f;
		base._Ready();
	}
}

using Godot;

public partial class EnemyBullet : BulletInheritance
{
	public override void _Ready()
	{
		base._Ready();
		selfGroup = "enemy_bullet";
		oppositeGroup = "player_bullet";
		//movementLogic = new BulletMovement(this, 1000f);
	}
	public override void Init(Vector2 direction, bool facingRight)
	{
		base.Init(direction, facingRight);
		AddToGroup("enemy_bullet");
	}
}

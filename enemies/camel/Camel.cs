using Godot;

public partial class Camel : EnemyManager
{
	protected override string BulletScenePath => "res://enemies/EnemyBullet.tscn";

	protected override void OnEnemyReady()
	{
		facingRight = true;
		bulletSpawnInterval = 3.0f;
		activationDistance = 500f;
	}
}

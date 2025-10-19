//using Godot;
//using System;
//
//public partial class Atlas : BaseCharacter
//{
	//public override void _Ready()
	//{
		//base._Ready();
		//GD.Print("Atlas ready!");
	//}
//
	//protected override void Shoot()
	//{
		//base.Shoot();
		//GD.Print("Atlas shoots a bullet!");
	//}
//
	//protected override void StartDash(Vector2 direction)
	//{
		//base.StartDash(direction);
		//GD.Print("Atlas dashes!");
	//}
//}
using Godot;
using System;

public partial class Atlas : BaseCharacter
{
	private AnimatedSprite2D characterSprite;
	private AnimatedSprite2D weapon;
	private Vector2 originalWeaponScale;
	private Vector2 originalCharacterScale;
	private Marker2D bulletSpawn;

	public override void _Ready()
	{
		base._Ready();

		characterSprite = GetNodeOrNull<AnimatedSprite2D>("Character2D");
		weapon = GetNodeOrNull<AnimatedSprite2D>("Weapon2D");
		bulletSpawn = weapon?.GetNodeOrNull<Marker2D>("bullet_spawn");

		if (characterSprite == null) GD.PrintErr("[Atlas] Missing Character2D");
		if (weapon == null) GD.PrintErr("[Atlas] Missing Weapon2D");
		if (bulletSpawn == null) GD.PrintErr("[Atlas] Missing bullet_spawn");

		if (weapon != null)
			originalWeaponScale = weapon.Scale;
		if (characterSprite != null)
			originalCharacterScale = characterSprite.Scale;
	}

	protected override void OnMove(Vector2 dir)
	{
		characterSprite?.Play("run");

		UpdateDirection(dir);
		RotateWeapon(dir);
	}

	protected override void OnIdle()
	{
		characterSprite?.Play("idle");
		// коли стоїть — повертай пушку в базову позицію
		if (weapon != null){
			if(facingRight){weapon.RotationDegrees = 0;}
			else {weapon.RotationDegrees = 180;}
		}
	}

	protected override void OnDashStart()
	{
		characterSprite?.Play("dash");
	}

	protected override void OnDashEnd()
	{
		characterSprite?.Play("idle");
	}

	protected override void OnShoot()
	{
		if (bulletBase == null || bulletSpawn == null)
			return;

		var bullet = bulletBase.Instantiate<StandartBullet>();
		bullet.GlobalPosition = bulletSpawn.GlobalPosition;
		bullet.Init(movement, facingRight);
		GetTree().CurrentScene.AddChild(bullet);
	}

	private void UpdateDirection(Vector2 movement)
	{
		if (movement.X < 0 && facingRight)
		{
			facingRight = false;
			characterSprite.FlipH = true;
			if (weapon != null)
				weapon.Scale = new Vector2(Mathf.Abs(originalWeaponScale.X), -Mathf.Abs(originalWeaponScale.Y));
		}
		else if (movement.X > 0 && !facingRight)
		{
			facingRight = true;
			characterSprite.FlipH = false;
			if (weapon != null)
				weapon.Scale = new Vector2(Mathf.Abs(originalWeaponScale.X), Mathf.Abs(originalWeaponScale.Y));
		}
	}

	private void RotateWeapon(Vector2 direction)
	{
		if (direction == Vector2.Zero || weapon == null)
			return;

		float angle = Mathf.Atan2(direction.Y, direction.X);
		weapon.RotationDegrees = Mathf.RadToDeg(angle);
	}
}

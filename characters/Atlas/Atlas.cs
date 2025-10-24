using Godot;
using System;

public partial class Atlas : BaseCharacter
{
	private AnimatedSprite2D characterSprite;
	private AnimatedSprite2D weapon;
	private Marker2D bulletSpawn;
	private Vector2 originalWeaponScale = Vector2.One;
	private Vector2 originalCharacterScale = Vector2.One;
	

	public override void _Ready()
	{
		base._Ready();
		Init(250f, 500f, 1.5f, 1.2f, 0.15f, 12, 2);
		characterSprite = GetNodeOrNull<AnimatedSprite2D>("Character2D");
		weapon = GetNodeOrNull<AnimatedSprite2D>("Weapon2D");
		bulletSpawn = GetNodeOrNull<Marker2D>("Weapon2D/bullet_spawn");

		if (characterSprite == null) GD.PrintErr("[Atlas] Missing Character2D");
		else characterSprite.Scale *= (float)1.3;
		if (weapon == null) GD.PrintErr("[Atlas] Missing Weapon2D");
		else weapon.Scale *= (float)1.3;
		if (bulletSpawn == null) GD.PrintErr("[Atlas] Missing Weapon2D/bullet_spawn");
		

		if (weapon != null) originalWeaponScale = weapon.Scale;
		if (characterSprite != null) originalCharacterScale = characterSprite.Scale;

		characterSprite?.Play("idle");
	}

	protected override bool Shoot()
	{
		if (bulletBase == null)
		{
			GD.PrintErr("[Atlas] Bullet scene not found!");
			return false;
		}

		var inst = bulletBase.Instantiate();
		if (!(inst is StandartBullet bullet))
		{
			GD.PrintErr("[Atlas] Instantiated object is not StandartBullet");
			return false;
		}
		
		Node parentForBullets = this;
		var currentScene = GetTree().CurrentScene;
		if (currentScene != null)
		{
			if (currentScene.HasNode("Bullets"))
				parentForBullets = currentScene.GetNode("Bullets");
			else
				parentForBullets = currentScene;
		}

		parentForBullets.AddChild(bullet);

		Vector2 spawnPos = (bulletSpawn != null) ? bulletSpawn.GlobalPosition : GlobalPosition;
		bullet.GlobalPosition = spawnPos;

		Vector2 dir = movement != Vector2.Zero ? movement : (facingRight ? Vector2.Right : Vector2.Left);
		bullet.Init(dir, facingRight);

		return true;
	}

	protected override void OnMove(Vector2 dir)
	{
		characterSprite?.Play("run");
		
		if (dir.X < 0 && facingRight)
		{
			facingRight = false;
			if (characterSprite != null) characterSprite.FlipH = true;
			if (weapon != null) weapon.Scale = new Vector2(-Mathf.Abs(originalWeaponScale.X), Mathf.Abs(originalWeaponScale.Y));
		}
		else if (dir.X > 0 && !facingRight)
		{
			facingRight = true;
			if (characterSprite != null) characterSprite.FlipH = false;
			if (weapon != null) weapon.Scale = new Vector2(Mathf.Abs(originalWeaponScale.X), Mathf.Abs(originalWeaponScale.Y));
		}
		// Поворот зброї
		if (weapon != null)
		{
			if (dir != Vector2.Zero)
			{
				float angle = Mathf.Atan2(dir.Y, dir.X);
				float deg = Mathf.RadToDeg(angle);
				if (!facingRight) deg += 180f;
				weapon.RotationDegrees = deg;
			}
		}

	}

	protected override void OnIdle()
	{
		characterSprite?.Play("idle");
		if (weapon != null)
		{
			weapon.RotationDegrees = 0;
			weapon.Scale = facingRight ? new Vector2(Mathf.Abs(originalWeaponScale.X), Mathf.Abs(originalWeaponScale.Y)) 
				: new Vector2(-Mathf.Abs(originalWeaponScale.X), Mathf.Abs(originalWeaponScale.Y));
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
}

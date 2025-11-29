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
		Init(250f, 500f, 1.5f, 1.2f, 0.15f, 12, 2f);
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

		Vector2 spawnPos = bulletSpawn != null ? bulletSpawn.GlobalPosition : GlobalPosition;
		bullet.GlobalPosition = spawnPos;
	
		BaseEnemy enemy = FindClosestEnemy();
		Vector2 shootDir;

		if (enemy != null)
		{
			shootDir = (enemy.GlobalPosition - spawnPos).Normalized();
			// використовуємо реальну різницю X, щоб не залежати від округлень низького X у нормалізованому векторі
			facingRight = (enemy.GlobalPosition.X - spawnPos.X) >= 0;
		}
		else
		{
			if (movement != Vector2.Zero)
			{
				shootDir = movement.Normalized();
				facingRight = movement.X >= 0;
			}
			else
			{
				shootDir = (facingRight ? Vector2.Right : Vector2.Left);
				// facingRight залишається як є
			}
		}

		// застосовуємо візуал відразу, щоб персонаж одразу дивився в потрібний бік
		UpdateFacingVisuals();

		bullet.Init(shootDir, facingRight);

		//facingRight = we
		
		//GD.Print($"shoot dir {shootDir}");
		//GD.Print($"spawn position {spawnPos}");
		//GD.Print($"enemy globalPosition {enemy.GlobalPosition}");
		//GD.Print($"{}");
		return true;
	}

	private void UpdateFacingVisuals()
	{
		if (characterSprite != null)
			characterSprite.FlipH = !facingRight;

		if (weapon != null)
		{
			// Підтримуємо початковий масштаб за знаком facingRight
			weapon.Scale = facingRight
				? new Vector2(Mathf.Abs(originalWeaponScale.X), Mathf.Abs(originalWeaponScale.Y))
				: new Vector2(-Mathf.Abs(originalWeaponScale.X), Mathf.Abs(originalWeaponScale.Y));
		}
	}


	protected override void OnMove(Vector2 dir)
	{
		characterSprite?.Play("run");

		// Встановлюємо facingRight відразу за знаком dir.X (якщо рух горизонтальний)
		if (dir.X != 0)
		{
			bool newFacing = dir.X >= 0;
			if (newFacing != facingRight)
			{
				facingRight = newFacing;
				UpdateFacingVisuals();
			}
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

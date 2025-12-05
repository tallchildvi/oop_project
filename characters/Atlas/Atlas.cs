/// <file>
/// <summary>
/// Atlas.cs - Player character implementation with weapon aiming and animation
/// </summary>
/// </file>

using Godot;
using System;

/// <summary>
/// Atlas character class that extends BaseCharacter with specific animations and shooting mechanics.
/// </summary>
/// <remarks>
/// Implements auto-aiming towards enemies, weapon rotation, and character sprite flipping.
/// Handles all visual aspects of the Atlas character including movement animations and dash effects.
/// </remarks>
public partial class Atlas : BaseCharacter
{
	/// <summary>Animated sprite for the character body.</summary>
	private AnimatedSprite2D characterSprite;
	
	/// <summary>Animated sprite for the weapon.</summary>
	private AnimatedSprite2D weapon;
	
	/// <summary>Marker indicating bullet spawn position.</summary>
	private Marker2D bulletSpawn;
	
	/// <summary>Original scale of the weapon sprite for proper flipping.</summary>
	private Vector2 originalWeaponScale = Vector2.One;
	
	/// <summary>Original scale of the character sprite for proper flipping.</summary>
	private Vector2 originalCharacterScale = Vector2.One;
	

	/// <summary>
	/// Initializes the Atlas character and sets up sprite references.
	/// </summary>
	/// <remarks>
	/// Configures Atlas-specific parameters: speed=250, dashSpeed=500, dashTime=1.5s, 
	/// dashReload=1.2s, bulletReload=0.15s, maxAmmo=12, chargeTime=2s.
	/// Also retrieves and validates all sprite nodes and scales them appropriately.
	/// </remarks>
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

	/// <summary>
	/// Executes the shooting logic with auto-aiming towards nearest enemy.
	/// </summary>
	/// <returns>True if bullet was successfully spawned; otherwise false.</returns>
	/// <remarks>
	/// Creates a bullet instance, determines shoot direction (prioritizes closest enemy, 
	/// then movement direction, then facing direction), updates weapon rotation, 
	/// and spawns the bullet at the weapon's bullet spawn marker.
	/// </remarks>
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
		if (weapon != null)
		{
			if (shootDir != Vector2.Zero)
			{
				float angle = Mathf.Atan2(shootDir.Y, shootDir.X);
				float deg = Mathf.RadToDeg(angle);
				GD.Print(angle);
				GD.Print(deg);
				if (!facingRight) deg += 180f;
				weapon.RotationDegrees = deg;
			}
		}
		
		GD.Print(shootDir);
		bullet.Init(shootDir, facingRight);
		//if (weapon != null)
		//{
			//weapon.RotationDegrees = 0f; 
		//}
		return true;
	}

	/// <summary>
	/// Updates character and weapon sprites based on facing direction.
	/// </summary>
	/// <remarks>
	/// Flips the character sprite horizontally and adjusts weapon scale 
	/// to maintain proper visual orientation.
	/// </remarks>
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


	/// <summary>
	/// Called when the character is moving. Updates animation and facing direction.
	/// </summary>
	/// <param name="dir">Movement direction vector.</param>
	/// <remarks>
	/// Plays the "run" animation and updates facing direction based on horizontal movement.
	/// </remarks>
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
		//if (weapon != null)
		//{
			//if (dir != Vector2.Zero)
			//{
				//float angle = Mathf.Atan2(dir.Y, dir.X);
				//float deg = Mathf.RadToDeg(angle);
				//if (!facingRight) deg += 180f;
				//weapon.RotationDegrees = deg;
			//}
		//}
	}


	/// <summary>
	/// Called when the character is idle. Resets animation and weapon rotation.
	/// </summary>
	/// <remarks>
	/// Plays the "idle" animation and smoothly rotates the weapon back to 0 degrees.
	/// Maintains proper weapon scale based on facing direction.
	/// </remarks>
	protected override void OnIdle()
	{
		characterSprite?.Play("idle");
		if (weapon != null)
		{
			// Нормалізуємо кут до діапазону -180 до 180
			float targetRotation = Mathf.Wrap(weapon.RotationDegrees, -180f, 180f);
			
			// Плавне повернення до 0
			float rotationSpeed = 5f;
			if (Mathf.Abs(targetRotation) > 0.1f)
			{
				// Визначаємо найкоротший шлях до 0
				if (targetRotation > 180f) targetRotation -= 360f;
				if (targetRotation < -180f) targetRotation += 360f;
				
				weapon.RotationDegrees = Mathf.MoveToward(targetRotation, 0, rotationSpeed);
			}
			else
			{
				weapon.RotationDegrees = 0;
			}
			
			weapon.Scale = facingRight 
				? new Vector2(Mathf.Abs(originalWeaponScale.X), Mathf.Abs(originalWeaponScale.Y)) 
				: new Vector2(-Mathf.Abs(originalWeaponScale.X), Mathf.Abs(originalWeaponScale.Y));
		}
	}

	/// <summary>
	/// Called when dash starts. Plays dash animation.
	/// </summary>
	protected override void OnDashStart()
	{
		characterSprite?.Play("dash");
	}

	/// <summary>
	/// Called when dash ends. Returns to idle animation.
	/// </summary>
	protected override void OnDashEnd()
	{
		characterSprite?.Play("idle");
	}
}
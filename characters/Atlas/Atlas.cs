////using Godot;
////using System;
////
////public partial class Atlas : BaseCharacter
////{
	////private AnimatedSprite2D characterSprite;
	////private AnimatedSprite2D weapon;
	////private Vector2 originalWeaponScale;
	////private Vector2 originalCharacterScale;
	////private Marker2D bulletSpawn;
////
	////public override void _Ready()
	////{
		////base._Ready();
////
		////characterSprite = GetNodeOrNull<AnimatedSprite2D>("Character2D");
		////weapon = GetNodeOrNull<AnimatedSprite2D>("Weapon2D");
		////bulletSpawn = weapon?.GetNodeOrNull<Marker2D>("bullet_spawn");
////
		////if (characterSprite == null) GD.PrintErr("[Atlas] Missing Character2D");
		////if (weapon == null) GD.PrintErr("[Atlas] Missing Weapon2D");
		////if (bulletSpawn == null) GD.PrintErr("[Atlas] Missing bullet_spawn");
////
		////if (weapon != null) originalWeaponScale = weapon.Scale;
		////if (characterSprite != null) originalCharacterScale = characterSprite.Scale;
	////}
////
	////protected override void OnMove(Vector2 dir)
	////{
		////characterSprite?.Play("run");
		////UpdateDirection(dir);
		////RotateWeapon(dir);
	////}
////
	////protected override void OnIdle()
	////{
		////characterSprite?.Play("idle");
		////if (weapon != null)
			////weapon.RotationDegrees = facingRight ? 0 : 180;
	////}
////
	////protected override void OnDashStart()
	////{
		////characterSprite?.Play("dash");
		////
	////}
////
	////protected override void OnDashEnd()
	////{
		////characterSprite?.Play("idle");
	////}
////
	////protected override void Shoot()
	////{
		////if (bulletBase == null || bulletSpawn == null)
		////{
			////GD.PrintErr("[Atlas] Bullet or spawn point missing");
			////return;
		////}
////
		////var bullet = bulletBase.Instantiate<StandartBullet>();
		////bullet.GlobalPosition = bulletSpawn.GlobalPosition;
////
		////Vector2 dir = movement != Vector2.Zero ? movement : (facingRight ? Vector2.Right : Vector2.Left);
		////bullet.Init(dir, facingRight);
////
		////GetTree().CurrentScene.AddChild(bullet);
	////}
////
////
	////private void UpdateDirection(Vector2 dir)
	////{
		////if (dir.X < 0 && facingRight)
		////{
			////facingRight = false;
			////if (characterSprite != null) characterSprite.FlipH = true;
			////if (weapon != null)
				////weapon.Scale = new Vector2(Mathf.Abs(originalWeaponScale.X), -Mathf.Abs(originalWeaponScale.Y));
		////}
		////else if (dir.X > 0 && !facingRight)
		////{
			////facingRight = true;
			////if (characterSprite != null) characterSprite.FlipH = false;
			////if (weapon != null)
				////weapon.Scale = new Vector2(Mathf.Abs(originalWeaponScale.X), Mathf.Abs(originalWeaponScale.Y));
		////}
	////}
////
	////private void RotateWeapon(Vector2 direction)
	////{
		////if (direction == Vector2.Zero || weapon == null) return;
////
		////float angle = Mathf.Atan2(direction.Y, direction.X);
		////weapon.RotationDegrees = Mathf.RadToDeg(angle);
	////}
////}
//using Godot;
//using System;
//
//public partial class Atlas : BaseCharacter
//{
	//private AnimatedSprite2D characterSprite;
	//private AnimatedSprite2D weapon;
	//private Marker2D bulletSpawn;
	//private Vector2 originalWeaponScale = Vector2.One;
//
	//public override void _Ready()
	//{
		//base._Ready();
//
		//characterSprite = GetNodeOrNull<AnimatedSprite2D>("Character2D");
		//weapon = GetNodeOrNull<AnimatedSprite2D>("Weapon2D");
		//bulletSpawn = GetNodeOrNull<Marker2D>("Weapon2D/bullet_spawn");
//
		//if (characterSprite == null) GD.PrintErr("[Atlas] Missing Character2D");
		//if (weapon == null) GD.PrintErr("[Atlas] Missing Weapon2D");
		//if (bulletSpawn == null) GD.PrintErr("[Atlas] Missing Weapon2D/bullet_spawn");
//
		//if (weapon != null) originalWeaponScale = weapon.Scale;
	//}
//
	//protected override void OnMove(Vector2 dir)
	//{
		//characterSprite?.Play("run");
		//UpdateDirection(dir);
		//RotateWeapon(dir);
	//}
//
	//protected override void OnIdle()
	//{
		//characterSprite?.Play("idle");
		//if (weapon != null)
			//weapon.RotationDegrees = facingRight ? 0 : 180;
	//}
//
	//protected override void OnDashStart()
	//{
		//characterSprite?.Play("dash");
	//}
//
	//protected override void OnDashEnd()
	//{
		//characterSprite?.Play("idle");
	//}
//
	//protected override void OnShoot()
	//{
		//// Тут лише візуальні ефекти/анімації/звук для стрільби
		//characterSprite?.Play("shoot"); // якщо є анімація
	//}
//
	//// Повертає реальну позицію спавну кулі
	//protected override Vector2 GetBulletSpawnPosition()
	//{
		//return bulletSpawn != null ? bulletSpawn.GlobalPosition : GlobalPosition;
	//}
//
	//private void UpdateDirection(Vector2 dir)
	//{
		//if (dir.X < 0 && facingRight)
		//{
			//facingRight = false;
			//if (characterSprite != null) characterSprite.FlipH = true;
			//if (weapon != null)
				//weapon.Scale = new Vector2(Mathf.Abs(originalWeaponScale.X), -Mathf.Abs(originalWeaponScale.Y));
		//}
		//else if (dir.X > 0 && !facingRight)
		//{
			//facingRight = true;
			//if (characterSprite != null) characterSprite.FlipH = false;
			//if (weapon != null)
				//weapon.Scale = new Vector2(Mathf.Abs(originalWeaponScale.X), Mathf.Abs(originalWeaponScale.Y));
		//}
	//}
//
	//private void RotateWeapon(Vector2 direction)
	//{
		//if (weapon == null) return;
//
		//if (direction == Vector2.Zero)
		//{
			//weapon.RotationDegrees = facingRight ? 0 : 180;
			//return;
		//}
//
		//float angle = Mathf.Atan2(direction.Y, direction.X);
		//float deg = Mathf.RadToDeg(angle);
//
		//// Якщо дивимось вліво — коригуємо орієнтацію
		//if (!facingRight)
			//deg += 180f;
//
		//weapon.RotationDegrees = deg;
	//}
//}
//using Godot;
//using System;
//
//public partial class Atlas : BaseCharacter
//{
	//private AnimatedSprite2D characterSprite;
	//private AnimatedSprite2D weapon;
	//private Marker2D bulletSpawn;
	//private Vector2 originalWeaponScale = Vector2.One;
//
	//public override void _Ready()
	//{
		//base._Ready();
//
		//characterSprite = GetNodeOrNull<AnimatedSprite2D>("Character2D");
		//weapon = GetNodeOrNull<AnimatedSprite2D>("Weapon2D");
		//bulletSpawn = GetNodeOrNull<Marker2D>("Weapon2D/bullet_spawn");
//
		//if (characterSprite == null) GD.PrintErr("[Atlas] Missing Character2D");
		//if (weapon == null) GD.PrintErr("[Atlas] Missing Weapon2D");
		//if (bulletSpawn == null) GD.PrintErr("[Atlas] Missing Weapon2D/bullet_spawn");
//
		//if (weapon != null) originalWeaponScale = weapon.Scale;
	//}
//
	//protected override void OnMove(Vector2 dir)
	//{
		//characterSprite?.Play("run");
		//UpdateDirection(dir);
		//RotateWeapon(dir);
	//}
//
	//protected override void OnIdle()
	//{
		//characterSprite?.Play("idle");
		//if (weapon != null)
			//weapon.RotationDegrees = facingRight ? 0 : 180;
	//}
//
	//protected override void OnDashStart()
	//{
		//characterSprite?.Play("dash");
	//}
//
	//protected override void OnDashEnd()
	//{
		//characterSprite?.Play("idle");
	//}
//
	//protected override void OnShoot()
	//{
		//// Тільки візуал (анімація/звук)
		//characterSprite?.Play("shoot"); // якщо є анімація
	//}
//
	//// Повертає реальну позицію спавну кулі
	//protected override Vector2 GetBulletSpawnPosition()
	//{
		//return bulletSpawn != null ? bulletSpawn.GlobalPosition : GlobalPosition;
	//}
//
	//private void UpdateDirection(Vector2 dir)
	//{
		//if (dir.X < 0 && facingRight)
		//{
			//facingRight = false;
			//if (characterSprite != null) characterSprite.FlipH = true;
			//if (weapon != null)
				//weapon.Scale = new Vector2(Mathf.Abs(originalWeaponScale.X), -Mathf.Abs(originalWeaponScale.Y));
		//}
		//else if (dir.X > 0 && !facingRight)
		//{
			//facingRight = true;
			//if (characterSprite != null) characterSprite.FlipH = false;
			//if (weapon != null)
				//weapon.Scale = new Vector2(Mathf.Abs(originalWeaponScale.X), Mathf.Abs(originalWeaponScale.Y));
		//}
	//}
//
	//private void RotateWeapon(Vector2 direction)
	//{
		//if (weapon == null) return;
//
		//if (direction == Vector2.Zero)
		//{
			//weapon.RotationDegrees = facingRight ? 0 : 180;
			//return;
		//}
//
		//float angle = Mathf.Atan2(direction.Y, direction.X);
		//float deg = Mathf.RadToDeg(angle);
//
		//if (!facingRight)
			//deg += 180f;
//
		//weapon.RotationDegrees = deg;
	//}
//}
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
		Init(250f, 500f, 0.25f, 1.2f, 0.35f, 12);
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

		// Куди додати кулю: шукаємо node "Bullets" у поточній сцені, інакше додаємо в корінь сцени
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

		// напрямок: якщо стоїмо — стріляємо у бік facingRight
		Vector2 dir = movement != Vector2.Zero ? movement : (facingRight ? Vector2.Right : Vector2.Left);
		bullet.Init(dir, facingRight);

		return true;
	}

	// Візуальна частина (hooks)
	protected override void OnMove(Vector2 dir)
	{
		characterSprite?.Play("run");

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
			else
			{
				weapon.RotationDegrees = facingRight ? 0 : 180;
			}
		}
		if (dir.X < 0 && facingRight)
		{
			facingRight = false;
			if (characterSprite != null) characterSprite.FlipH = true;
			if (weapon != null) weapon.Scale = new Vector2(Mathf.Abs(originalWeaponScale.X), -Mathf.Abs(originalWeaponScale.Y));
		}
		else if (dir.X > 0 && !facingRight)
		{
			facingRight = true;
			if (characterSprite != null) characterSprite.FlipH = false;
			if (weapon != null) weapon.Scale = new Vector2(Mathf.Abs(originalWeaponScale.X), Mathf.Abs(originalWeaponScale.Y));
		}
	}

	protected override void OnIdle()
	{
		characterSprite?.Play("idle");
		if (weapon != null) weapon.RotationDegrees = facingRight ? 0 : 180;
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

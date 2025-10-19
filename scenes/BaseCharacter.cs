//using Godot;
//using System;
//
//public partial class BaseCharacter : Area2D
//{
	//[Export] protected float speed = 200f;
	//[Export] protected float dashSpeed = 400f;
	//[Export] protected float dashTime = 0.2f;
	//[Export] protected float dashReloadTime = 1f;
	//[Export] protected float bulletReloadTime = 0.4f;
//
	//protected Vector2 movement = Vector2.Zero;
	//protected Vector2 dashDirection;
	//protected bool inDash = false;
	//protected float dashTimer = 0f;
	//protected float dashReloadTimer = 0f;
	//protected float bulletReloadTimer = 0f;
	//protected bool facingRight = true;
//
	//protected AnimatedSprite2D weapon;
	//protected AnimatedSprite2D characterSprite;
	//protected PackedScene bulletBase;
	//protected Vector2 originalWeaponScale;
	//protected Vector2 originalCharacterScale;
	//
	//protected bool control = true;
//
	//public override void _Ready()
	//{
		//weapon = GetNode<AnimatedSprite2D>("Atlas/Weapon2D");
		//if (characterSprite == null){GD.Print("weapon is null");}
		//characterSprite = GetNode<AnimatedSprite2D>("Atlas/Character2D");
		//if (characterSprite == null){GD.Print("characterSprite is null");}
		//bulletBase = ResourceLoader.Load<PackedScene>("res://Characters/Atlas/standart_bullet.tscn");
//
		//originalWeaponScale = weapon.Scale;
		//originalCharacterScale = characterSprite.Scale;
//
		//characterSprite.FlipH = false;
		//weapon.Scale = new Vector2(Mathf.Abs(originalWeaponScale.X), originalWeaponScale.Y);
		//facingRight = true;
//
		//characterSprite.Play("idle");
	//}
//
	//public override void _Process(double delta)
	//{
		//if (!control) return;
//
		//HandleMovement(delta);
		//HandleDash(delta);
		//HandleReloadTimers(delta);
		//HandleIdleState();
	//}
//
	//protected virtual void HandleMovement(double delta)
	//{
		//if (inDash)
			//return;
//
		//movement = Vector2.Zero;
//
		//if (Input.IsKeyPressed(Key.W)) movement.Y -= 1;
		//if (Input.IsKeyPressed(Key.S)) movement.Y += 1;
		//if (Input.IsKeyPressed(Key.A)) movement.X -= 1;
		//if (Input.IsKeyPressed(Key.D)) movement.X += 1;
//
		//if (movement != Vector2.Zero)
		//{
			//movement = movement.Normalized();
			//Position += movement * speed * (float)delta;
			//float angle = Mathf.Atan2(movement.Y, movement.X);
			//weapon.RotationDegrees = Mathf.RadToDeg(angle);
//
			//UpdateDirection();
			//characterSprite.Play("run");
		//}
//
		//if (Input.IsKeyPressed(Key.Shift) && dashReloadTimer <= 0f && movement != Vector2.Zero)
			//StartDash(movement);
//
		//if (Input.IsKeyPressed(Key.J))
			//TryShoot();
	//}
//
	//protected virtual void UpdateDirection()
	//{
		//// Дзеркалення персонажа і зброї
		//if (movement.X < 0 && facingRight)
		//{
			//facingRight = false;
			//characterSprite.FlipH = true;
			//weapon.Scale = new Vector2(Mathf.Abs(originalWeaponScale.X), -Mathf.Abs(originalWeaponScale.Y));
		//}
		//else if (movement.X > 0 && !facingRight)
		//{
			//facingRight = true;
			//characterSprite.FlipH = false;
			//weapon.Scale = new Vector2(Mathf.Abs(originalWeaponScale.X), originalWeaponScale.Y);
		//}
	//}
//
	//protected virtual void HandleDash(double delta)
	//{
		//if (inDash)
		//{
			//Position += dashDirection * dashSpeed * (float)delta;
			//dashTimer -= (float)delta;
//
			//if (dashTimer <= 0f)
			//{
				//inDash = false;
				//dashReloadTimer = dashReloadTime;
				//characterSprite.Play("idle");
			//}
		//}
	//}
//
	//protected virtual void HandleReloadTimers(double delta)
	//{
		//if (dashReloadTimer > 0f) dashReloadTimer -= (float)delta;
		//if (bulletReloadTimer > 0f) bulletReloadTimer -= (float)delta;
	//}
//
	//protected virtual void HandleIdleState()
	//{
		//if (!inDash && movement == Vector2.Zero)
			//characterSprite.Play("idle");
	//}
//
	//protected virtual void StartDash(Vector2 direction)
	//{
		//inDash = true;
		//dashDirection = direction.Normalized();
		//dashTimer = dashTime;
		//characterSprite.Play("dash");
	//}
//
	//protected virtual void TryShoot()
	//{
		//if (bulletReloadTimer > 0f || bulletBase == null)
			//return;
//
		//Shoot();
		//bulletReloadTimer = bulletReloadTime;
	//}
	//
	//protected virtual void Shoot()
	//{
		//var bullet = bulletBase.Instantiate<StandartBullet>();
		//var spawnMarker = GetNode<Marker2D>("Weapon2D/bullet_spawn");
		//Vector2 spawnPos = spawnMarker.GlobalPosition;
		//bullet.Init(movement, facingRight);
		//GetTree().CurrentScene.AddChild(bullet);
		//bullet.GlobalPosition = spawnPos;
	//}
//
	//public virtual void EnableControl() => control = true;
	//public virtual void DisableControl() => control = false;
//}
using Godot;
using System;

public partial class BaseCharacter : Area2D
{
	protected float speed = 200f;
	protected float dashSpeed = 400f;
	protected float dashTime = 0.2f;
	protected float dashReloadTime = 1f;
	protected float bulletReloadTime = 0.4f;
	protected int health = 5;

	protected Vector2 movement = Vector2.Zero;
	protected Vector2 dashDirection;
	protected bool inDash = false;
	protected float dashTimer = 0f;
	protected float dashReloadTimer = 0f;
	protected float bulletReloadTimer = 0f;
	protected bool facingRight = true;
	protected bool control = true;

	protected PackedScene bulletBase;

	public override void _Ready()
	{
		//change path
		bulletBase = ResourceLoader.Load<PackedScene>("res://Characters/Atlas/standart_bullet.tscn");
	}

	public override void _Process(double delta)
	{
		//if (!control) return;
		AddToGroup("player");
		HandleMovement(delta);
		HandleDash(delta);
		HandleReloadTimers(delta);
		HandleIdleState();
	}

	protected virtual void HandleMovement(double delta)
	{
		if (inDash)
			return;

		movement = Vector2.Zero;

		if (Input.IsKeyPressed(Key.W)) movement.Y -= 1;
		if (Input.IsKeyPressed(Key.S)) movement.Y += 1;
		if (Input.IsKeyPressed(Key.A)) movement.X -= 1;
		if (Input.IsKeyPressed(Key.D)) movement.X += 1;

		if (movement != Vector2.Zero)
		{
			movement = movement.Normalized();
			Position += movement * speed * (float)delta;
			OnMove(movement);
		}

		if (Input.IsKeyPressed(Key.Shift) && dashReloadTimer <= 0f && movement != Vector2.Zero)
			StartDash(movement);

		if (Input.IsKeyPressed(Key.J))
			TryShoot();
	}

	protected virtual void HandleDash(double delta)
	{
		if (!inDash) return;

		Position += dashDirection * dashSpeed * (float)delta;
		dashTimer -= (float)delta;

		if (dashTimer <= 0f)
		{
			inDash = false;
			dashReloadTimer = dashReloadTime;
			OnDashEnd();
		}
	}

	protected virtual void HandleReloadTimers(double delta)
	{
		if (dashReloadTimer > 0f) dashReloadTimer -= (float)delta;
		if (bulletReloadTimer > 0f) bulletReloadTimer -= (float)delta;
	}

	protected virtual void HandleIdleState()
	{
		if (!inDash && movement == Vector2.Zero)
			OnIdle();
	}

	protected virtual void StartDash(Vector2 direction)
	{
		inDash = true;
		dashDirection = direction.Normalized();
		dashTimer = dashTime;
		OnDashStart();
	}

	protected virtual void TryShoot()
	{
		if (bulletReloadTimer > 0f || bulletBase == null)
			return;

		Shoot();
		bulletReloadTimer = bulletReloadTime;
	}

	protected virtual void Shoot()
	{
		OnShoot();
	}
	public void TakeDamage(int damage)
	{
		if(health - damage > 0){
			health -= damage;
			GD.Print("[Player] take damage");
		}
		else{
			GD.Print("[Player] take damage (0)");
		}
	}
	protected virtual void OnMove(Vector2 dir) { }
	protected virtual void OnIdle() { }
	protected virtual void OnDashStart() { }
	protected virtual void OnDashEnd() { }
	protected virtual void OnShoot() { }

	public virtual void EnableControl() => control = true;
	public virtual void DisableControl() => control = false;
}

using Godot;
using System;

public partial class BaseCharacter : Area2D
{
	public float speed;
	protected float dashSpeed;
	protected float dashTime;
	protected float dashReloadTime;
	protected float bulletReloadTime;
	protected int maxHealth = 5;
	protected int maxAmmo = 10;
	protected int ammo = 0;
	protected PackedScene bulletBase; 
	protected Vector2 movement = Vector2.Zero;
	protected Vector2 dashDirection = Vector2.Zero;
	protected bool inDash = false;
	protected float dashTimer = 0f;
	protected float dashReloadTimer = 0f;
	protected float bulletReloadTimer = 0f;
	protected bool facingRight = true;
	protected int health;
	protected bool control = true;
	protected float chargeTime; 
	private bool isCharging = false;
	private float chargeTimer;
	private IInput input;

	public override void _Ready()
	{
		AddToGroup("player");
		if (bulletBase == null)
			bulletBase = ResourceLoader.Load<PackedScene>("res://characters/Atlas/standart_bullet.tscn");

		health = maxHealth;
		ammo = maxAmmo;
	}

	public void Init(float set_speed = 200f, float set_dashSpeed = 400f, float set_dashTime = 0.2f, 
		float set_dashReloadTime = 1f, float set_bulletReloadTime = 0.4f, int set_maxAmmo = 10, float set_chargeTime = 3f)
	{	
		GD.Print("try to init character");
		speed = set_speed;
		dashSpeed = set_dashSpeed;
		dashTime = set_dashTime;
		dashReloadTime = set_dashReloadTime;
		bulletReloadTime = set_bulletReloadTime;
		maxAmmo = set_maxAmmo;
		ammo = set_maxAmmo;
		chargeTime = set_chargeTime;
	}
	public void SetInput(IInput newInput)
	{
		input = newInput;
	}
	public override void _Process(double delta)
	{
		if (!control) return;

		if (input != null)
		{
			movement = input.GetMovement();
			if (input.ToDash() && dashReloadTimer <= 0f && movement != Vector2.Zero && !inDash)
				StartDash(movement);
			if (input.ToShoot())
				TryShoot();
		}

		if (dashReloadTimer > 0f) dashReloadTimer = Math.Max(0f, dashReloadTimer - (float)delta);
		if (bulletReloadTimer > 0f) bulletReloadTimer = Math.Max(0f, bulletReloadTimer - (float)delta);
		if (chargeTimer > 0f) chargeTimer = Math.Max(0f, chargeTimer - (float)delta);
		if (isCharging && chargeTimer <= 0f)
		{
			isCharging = false;
			SetAmmo(maxAmmo);
			GD.Print($"weapon is charged to {ammo}");
		}
		if (inDash)
		{
			Position += dashDirection * dashSpeed * (float)delta;
			dashTimer -= (float)delta;
			if (dashTimer <= 0f)
			{
				inDash = false;
				dashReloadTimer = dashReloadTime;
				OnDashEnd();
			}
		}
		else
		{
			if (movement != Vector2.Zero)
			{
				movement = movement.Normalized();
				Position += movement * speed * (float)delta;
				OnMove(movement);
				//GD.Print($"Speed: {speed}");
				//GD.Print($"Movement: {movement}");
				//GD.Print($"Position: {Position}");
				//GD.Print($"delta: {delta}");
				//GD.Print($"expected change of position: {movement * speed}");
			}
			else
			{
				OnIdle();
			}
		}
	}

	protected virtual void StartDash(Vector2 direction)
	{
		inDash = true;
		dashDirection = direction.Normalized();
		dashTimer = dashTime;
		
		movement = Vector2.Zero;
		OnDashStart();
	}

	protected virtual void TryShoot()
	{
		if (bulletReloadTimer > 0f) return;
		if (bulletBase == null) return;
		if (ammo <= 0)
		{
			if (!isCharging) StartCharge();
			return;
		}

		if (Shoot())
		{
			bulletReloadTimer = bulletReloadTime;
			ConsumeAmmo(1);
			OnShoot();
		}
	}

	protected virtual bool Shoot()
	{
		return false;
	}

	public int GetAmmo() => ammo;
	public int GetMaxAmmo() => maxAmmo;

	public void StartCharge()
	{
		isCharging = true;
		chargeTimer = chargeTime;
	}

	public void SetAmmo(int value)
	{
		ammo = Mathf.Clamp(value, 0, maxAmmo);
	}

	protected void ConsumeAmmo(int amount)
	{
		ammo = Mathf.Clamp(ammo - amount, 0, maxAmmo);
		GD.Print($"[BaseCharacter] ammo: {ammo}");
	}

	protected virtual void OnMove(Vector2 dir) { }
	protected virtual void OnIdle() { }
	protected virtual void OnDashStart() { }
	protected virtual void OnDashEnd() { }
	protected virtual void OnShoot() { }

	public virtual void TakeDamage(int damage)
	{
		health -= damage;
		if (health > 0)
			GD.Print("[BaseCharacter] take damage");
		else
		{
			health = 0;
			GD.Print("[BaseCharacter] Died");
			EventManager.TriggerEvent("GAME_OVER", this);
		}
	}

	public void EnableControl() => control = true;
	public void DisableControl() => control = false;
}

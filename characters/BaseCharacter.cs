/// <file>
/// <summary>
/// BaseCharacter.cs - Base class for all game characters with movement, combat, and health management
/// </summary>
/// </file>

using Godot;
using System;

/// <summary>
/// Base class for all game characters. Provides movement, shooting, dashing, and health management functionality.
/// </summary>
/// <remarks>
/// This class serves as an abstract base for all game characters, including the player and enemies.
/// Inherits from Area2D for collision handling.
/// </remarks>
public partial class BaseCharacter : Area2D
{
	/// <summary>Normal movement speed of the character.</summary>
	public float speed;
	
	/// <summary>Movement speed during dash execution.</summary>
	protected float dashSpeed;
	
	/// <summary>Duration of the dash in seconds.</summary>
	protected float dashTime;
	
	/// <summary>Cooldown time before the next dash can be performed.</summary>
	protected float dashReloadTime;
	
	/// <summary>Cooldown time between shots.</summary>
	protected float bulletReloadTime;
	
	/// <summary>Maximum health of the character.</summary>
	protected int maxHealth = 5;
	
	/// <summary>Maximum ammunition capacity.</summary>
	protected int maxAmmo = 10;
	
	/// <summary>Current ammunition count.</summary>
	protected int ammo = 0;
	
	/// <summary>Scene template for creating bullets.</summary>
	protected PackedScene bulletBase; 
	
	/// <summary>Movement direction vector of the character.</summary>
	protected Vector2 movement = Vector2.Zero;
	
	/// <summary>Dash direction vector.</summary>
	protected Vector2 dashDirection = Vector2.Zero;
	
	/// <summary>Flag indicating if dash is currently active.</summary>
	protected bool inDash = false;
	
	/// <summary>Timer for current dash duration.</summary>
	protected float dashTimer = 0f;
	
	/// <summary>Timer for dash cooldown.</summary>
	protected float dashReloadTimer = 0f;
	
	/// <summary>Timer for weapon reload cooldown.</summary>
	protected float bulletReloadTimer = 0f;
	
	/// <summary>Flag for character facing direction (true = right).</summary>
	protected bool facingRight = true;
	
	/// <summary>Current health amount.</summary>
	protected int health;
	
	/// <summary>Flag for character control (true = controllable).</summary>
	protected bool control = true;
	
	/// <summary>Time required for full weapon charge.</summary>
	protected float chargeTime; 
	
	/// <summary>Flag indicating if charging is in progress.</summary>
	private bool isCharging = false;
	
	/// <summary>Timer for weapon charging process.</summary>
	private float chargeTimer;
	
	/// <summary>Interface for receiving input from player or AI.</summary>
	private IInput input;
	
	/// <summary>Maximum distance for automatic targeting.</summary>
	protected float autoAttackRange = 600f; 


	/// <summary>
	/// Called when the node enters the scene tree.
	/// </summary>
	/// <remarks>
	/// Initializes health, ammunition, loads bullet template, and triggers UI update events.
	/// </remarks>
	public override void _Ready()
	{
		AddToGroup("player");
		if (bulletBase == null)
			bulletBase = ResourceLoader.Load<PackedScene>("res://characters/Atlas/standart_bullet.tscn");

		health = maxHealth;
		ammo = maxAmmo;
		
		EventManager.TriggerEvent("PLAYER_HEALTH_CHANGED", new HealthData
		{
			CurrentHealth = health,
			MaxHealth = maxHealth
		});
		
		EventManager.TriggerEvent("PLAYER_AMMO_CHANGED", new AmmoData
		{
			CurrentAmmo = ammo,
			MaxAmmo = maxAmmo
		});
	}

	/// <summary>
	/// Initializes character parameters.
	/// </summary>
	/// <param name="set_speed">Movement speed (default 200).</param>
	/// <param name="set_dashSpeed">Dash speed (default 400).</param>
	/// <param name="set_dashTime">Dash duration (default 0.2 sec).</param>
	/// <param name="set_dashReloadTime">Dash cooldown time (default 1 sec).</param>
	/// <param name="set_bulletReloadTime">Weapon reload time (default 0.4 sec).</param>
	/// <param name="set_maxAmmo">Maximum ammunition capacity (default 10).</param>
	/// <param name="set_chargeTime">Weapon charge time (default 3 sec).</param>
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
	
	/// <summary>
	/// Sets the input source for the character.
	/// </summary>
	/// <param name="newInput">Object implementing the IInput interface.</param>
	public void SetInput(IInput newInput)
	{
		input = newInput;
	}
	
	/// <summary>
	/// Called every frame to process character logic.
	/// </summary>
	/// <param name="delta">Time elapsed since the previous frame in seconds.</param>
	/// <remarks>
	/// Handles input, movement, dashing, shooting, and updates timers.
	/// </remarks>
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
		//GD.Print(FindClosestEnemy().GlobalPosition);
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

	/// <summary>
	/// Initiates a dash in the specified direction.
	/// </summary>
	/// <param name="direction">Dash direction vector.</param>
	/// <remarks>
	/// Can be overridden in derived classes for additional logic.
	/// </remarks>
	protected virtual void StartDash(Vector2 direction)
	{
		inDash = true;
		dashDirection = direction.Normalized();
		dashTimer = dashTime;
		
		movement = Vector2.Zero;
		OnDashStart();
	}

	/// <summary>
	/// Attempts to perform a shot.
	/// </summary>
	/// <remarks>
	/// Checks reload timer and ammunition availability. 
	/// If ammunition is depleted, initiates charging.
	/// </remarks>
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

	/// <summary>
	/// Executes shooting logic. Must be overridden in derived classes.
	/// </summary>
	/// <returns>True if shot is successful; otherwise false.</returns>
	protected virtual bool Shoot()
	{
		return false;
	}

	/// <summary>
	/// Returns the current ammunition count.
	/// </summary>
	/// <returns>Ammunition count.</returns>
	public int GetAmmo() => ammo;
	
	/// <summary>
	/// Returns the maximum ammunition capacity.
	/// </summary>
	/// <returns>Maximum ammunition capacity.</returns>
	public int GetMaxAmmo() => maxAmmo;

	/// <summary>
	/// Initiates the weapon charging process.
	/// </summary>
	/// <remarks>
	/// After charging completes, ammunition will be fully restored.
	/// </remarks>
	public void StartCharge()
	{
		isCharging = true;
		chargeTimer = chargeTime;
	}

	/// <summary>
	/// Sets the ammunition count.
	/// </summary>
	/// <param name="value">New ammunition count.</param>
	/// <remarks>
	/// Value is automatically clamped to the range [0, maxAmmo].
	/// Triggers UI update event.
	/// </remarks>
	public void SetAmmo(int value)
	{
		ammo = Mathf.Clamp(value, 0, maxAmmo);
		EventManager.TriggerEvent("PLAYER_AMMO_CHANGED", new AmmoData
		{
			CurrentAmmo = ammo,
			MaxAmmo = maxAmmo
		});
	}

	/// <summary>
	/// Consumes the specified amount of ammunition.
	/// </summary>
	/// <param name="amount">Amount of ammunition to consume.</param>
	/// <remarks>
	/// Ammunition count cannot become negative.
	/// Triggers UI update event.
	/// </remarks>
	protected void ConsumeAmmo(int amount)
	{
		ammo = Mathf.Clamp(ammo - amount, 0, maxAmmo);
		GD.Print($"[BaseCharacter] ammo: {ammo}");
		EventManager.TriggerEvent("PLAYER_AMMO_CHANGED", new AmmoData
		{
			CurrentAmmo = ammo,
			MaxAmmo = maxAmmo
		});
	}

	/// <summary>
	/// Called when the character is moving. Can be overridden.
	/// </summary>
	/// <param name="dir">Movement direction.</param>
	protected virtual void OnMove(Vector2 dir) { }
	
	/// <summary>
	/// Called when the character is idle. Can be overridden.
	/// </summary>
	protected virtual void OnIdle() { }
	
	/// <summary>
	/// Called at the start of a dash. Can be overridden.
	/// </summary>
	protected virtual void OnDashStart() { }
	
	/// <summary>
	/// Called at the end of a dash. Can be overridden.
	/// </summary>
	protected virtual void OnDashEnd() { }
	
	/// <summary>
	/// Called after shooting. Can be overridden.
	/// </summary>
	protected virtual void OnShoot() { }

	/// <summary>
	/// Finds the closest living enemy within auto-attack range.
	/// </summary>
	/// <returns>The closest enemy, or null if no enemies found.</returns>
	/// <remarks>
	/// Searches all nodes in the "enemy" group and calculates distances from the character.
	/// Ignores dead enemies.
	/// </remarks>
	protected BaseEnemy FindClosestEnemy()
	{
		var enemies = GetTree().GetNodesInGroup("enemy");
		
		BaseEnemy closestEnemy = null;
		float closestDistance = autoAttackRange;

		foreach (Node node in enemies) 
		{
			BaseEnemy enemy = node as BaseEnemy; 
			if (enemy == null || enemy.Dead())
				continue;

			float dist = GlobalPosition.DistanceTo(enemy.GlobalPosition);
			if (dist < closestDistance)
			{
				closestDistance = dist;
				closestEnemy = enemy;
			}
		}

		return closestEnemy;
	}


	/// <summary>
	/// Applies damage to the character.
	/// </summary>
	/// <param name="damage">Amount of damage to apply.</param>
	/// <remarks>
	/// Reduces health and triggers health update event.
	/// If health reaches zero, triggers game over event.
	/// </remarks>
	public virtual void TakeDamage(int damage)
	{
		health -= damage;
		EventManager.TriggerEvent("PLAYER_HEALTH_CHANGED", new HealthData
		{
			CurrentHealth = health,
			MaxHealth = maxHealth
		});
		if (health > 0)
			GD.Print("[BaseCharacter] take damage");
		else
		{
			health = 0;
			GD.Print("[BaseCharacter] Died");
			EventManager.TriggerEvent("GAME_OVER", this);
		}
		
	}

	/// <summary>
	/// Enables character control.
	/// </summary>
	public void EnableControl() => control = true;
	
	/// <summary>
	/// Disables character control.
	/// </summary>
	public void DisableControl() => control = false;
}

/// <summary>
/// Data structure for health information.
/// </summary>
/// <remarks>
/// Used for passing health data through event system.
/// </remarks>
public class HealthData
{
	/// <summary>Current health value.</summary>
	public float CurrentHealth { get; set; }
	
	/// <summary>Maximum health value.</summary>
	public float MaxHealth { get; set; }
}

/// <summary>
/// Data structure for ammunition information.
/// </summary>
/// <remarks>
/// Used for passing ammunition data through event system.
/// </remarks>
public class AmmoData
{
	/// <summary>Current ammunition count.</summary>
	public float CurrentAmmo { get; set; }
	
	/// <summary>Maximum ammunition capacity.</summary>
	public float MaxAmmo { get; set; }
}
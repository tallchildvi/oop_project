using Godot;
using System;

public partial class BaseEnemy : Area2D
{
	public int maxHealth = 2;
	public float speed = 50f;
	protected int currentHealth;
	protected bool isDead;
	protected bool isPaused = false;
	protected bool facingRight = true;
	public BaseCharacter Player { get; protected set; }
	public event Action<BaseEnemy> Died;
	protected Vector2 movement = Vector2.Zero;

	public override void _Ready()
	{
		currentHealth = maxHealth;
		//AddToGroup("enemy");
		
		Player = GetTree().GetFirstNodeInGroup("player") as BaseCharacter;
		ProcessMode = ProcessModeEnum.Inherit;
	}
	public bool Dead(){
		return isDead;
	}
	public override void _Process(double delta){
		if (isPaused) return; 
		if (Player == null) return;
		movement = new Vector2(Player.GlobalPosition.X - Position.X, Player.GlobalPosition.Y - Position.Y);
		movement = movement.Normalized();
		Position += movement * speed * (float) delta;
	}
	public virtual void TakeDamage(int amount)
	{
		if (isDead) return;

		currentHealth -= amount;
		if (currentHealth <= 0)
			Die();
	}

	protected virtual void Die()
	{
		if (isDead) return;
		isDead = true; 
		try
		{
			Died?.Invoke(this);
		}
		catch (Exception ex)
		{
			GD.PrintErr($"[BaseEnemy] Exception while invoking Died: {ex}");
		}
		var collision = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
		if (collision != null)
			collision.SetDeferred("disabled", true);

		SetDeferred("visible", false);
		SetDeferred("process_mode", (int)ProcessModeEnum.Disabled);
		Visible = false;
		ProcessMode = ProcessModeEnum.Disabled;
	}

	public virtual void Activate(Vector2 spawnPosition)
	{
		AddToGroup("enemy");
		Position = spawnPosition;
		Visible = true;
		isDead = false;
		currentHealth = maxHealth;
		facingRight = true;
		ProcessMode = ProcessModeEnum.Inherit;

		var collision = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
		if (collision != null) collision.SetDeferred("disabled", false);;
	}

	public virtual void ResetState()
	{
		currentHealth = maxHealth;
		isDead = false;
		isPaused = false;
		Visible = true;
		facingRight = true;
		ProcessMode = ProcessModeEnum.Inherit;

		var collision = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
		if (collision != null) collision.SetDeferred("disabled", false);
	}

	public virtual void Pause()
	{
		isPaused = true;
		ProcessMode = ProcessModeEnum.Disabled;
	}

	public virtual void Resume()
	{
		isPaused = false;
		ProcessMode = ProcessModeEnum.Inherit;
	}
}

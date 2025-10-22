using Godot;
using System;

public partial class BaseEnemy : Area2D
{
	[Export] public int MaxHealth = 100;
	[Export] public float Speed = 100f;

	protected int currentHealth;
	protected bool isDead;
	protected bool isPaused = false;

	// напрямок, щоб ворог знав куди "дивитись"
	protected bool FacingRight = true;

	// посилання на гравця (може встановити EnemyManager або знайти у сцені)
	public BaseCharacter Player { get; protected set; }

	// Подія смерті — EnemyManager підписується на неї
	public event Action<BaseEnemy> Died;

	public override void _Ready()
	{
		currentHealth = MaxHealth;
		AddToGroup("enemy");
		
		Player = GetTree().GetFirstNodeInGroup("player") as BaseCharacter;
		ProcessMode = ProcessModeEnum.Inherit;
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

	// Викликає пул/менеджер, щоб активувати ворога (встановити позицію, включити колізії)
	public virtual void Activate(Vector2 spawnPosition)
	{
		Position = spawnPosition;
		Visible = true;
		isDead = false;
		currentHealth = MaxHealth;
		FacingRight = true;
		ProcessMode = ProcessModeEnum.Inherit;

		var collision = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
		if (collision != null) collision.Disabled = false;
	}

	// Викликається коли ворога повертають в пул або треба скинути внутрішній стан
	public virtual void ResetState()
	{
		currentHealth = MaxHealth;
		isDead = false;
		isPaused = false;
		Visible = true;
		FacingRight = true;
		ProcessMode = ProcessModeEnum.Inherit;

		var collision = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
		if (collision != null) collision.Disabled = false;
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

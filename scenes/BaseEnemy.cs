using Godot;
using System;

public abstract partial class BaseEnemy : Area2D
{
	[Export] public float Speed = 60f;
	[Export] public float HP = 3f;

	protected BaseCharacter Player;
	protected bool IsPaused = false;
	protected bool FacingRight = true;

	public event Action<BaseEnemy> Died;

	public override void _Ready()
	{
		AddToGroup("enemy");
		Player = GetTree().GetFirstNodeInGroup("player") as BaseCharacter;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (IsPaused || Player == null) return;
	}
	public virtual void TakeDamage(float dmg)
	{
		HP -= dmg;
		if (HP <= 0)
			Die();
	}

	protected virtual void Die()
	{
		Died?.Invoke(this);
		QueueFree();
	}

	public virtual void Pause() => IsPaused = true;
	public virtual void Resume() => IsPaused = false;
	public virtual void ResetState() {}
}

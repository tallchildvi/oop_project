using Godot;
using System;

public partial class Enemy : Node2D
{
	[Export] public float Speed = 60f;
	[Export] public float HP = 3f;

	private Player _player;
	private bool _isPaused = false;

	public event Action<Enemy> Died;

	public override void _Ready()
	{
		//_player = GetTree().Root.GetNode<Player>("main_character"); // шукати свого гравця
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_isPaused || _player == null) return;

		var direction = (_player.GlobalPosition - GlobalPosition).Normalized();
		//Velocity = direction * Speed;
		//MoveAndSlide();
	}

	public void TakeDamage(float dmg)
	{
		HP -= dmg;
		if (HP <= 0)
		{
			Die();
		}
	}

	private void Die()
	{
		Died?.Invoke(this);
		QueueFree();
	}

	public void Pause() => _isPaused = true;
	public void Resume() => _isPaused = false;
	public void ResetState(){}
}

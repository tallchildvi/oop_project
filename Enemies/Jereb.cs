using Godot;
using System;

public partial class Jereb : BaseEnemy
{
	[Export] private float bulletSpawnInterval = 3.0f;
	[Export] private PackedScene bulletScene;
	[Export] private float activationDistance = 400f;

	private AnimatedSprite2D characterSprite;
	private Timer _bulletTimer;
	private DistanceChecker _distanceChecker;
	private BulletSpawner _bulletSpawner;

	public override void _Ready()
	{
		base._Ready();

		characterSprite = GetNodeOrNull<AnimatedSprite2D>("EnemySprite");
		if (characterSprite == null)
			GD.PrintErr("[Jereb] Missing AnimatedSprite2D 'EnemySprite' (optional, but recommended)");

		_distanceChecker = new DistanceChecker(this, activationDistance);
		_bulletSpawner = new BulletSpawner(this, bulletScene, "enemy_bullet");

		_bulletTimer = new Timer
		{
			OneShot = false,
			WaitTime = bulletSpawnInterval
		};
		AddChild(_bulletTimer);
		_bulletTimer.Timeout += OnBulletTimerTimeout;
		_bulletTimer.Stop(); 

		AddToGroup("enemy");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (isDead) return; 

		if (Player == null)
		{
			Player = GetTree().GetFirstNodeInGroup("player") as BaseCharacter;
			if (Player == null) return;
		}
		_distanceChecker.CheckDistance(Player.GlobalPosition);

		if (_distanceChecker.IsActive)
		{
			if (_bulletTimer.IsStopped())
			{
				_bulletTimer.WaitTime = bulletSpawnInterval;
				_bulletTimer.Start();
			}
		}
		else
		{
			if (!_bulletTimer.IsStopped())
				_bulletTimer.Stop();
		}

		UpdateFacingTowardsPlayer();
	}

	private void UpdateFacingTowardsPlayer()
	{
		if (Player == null || characterSprite == null) return;

		bool shouldFaceRight = Player.GlobalPosition.X > GlobalPosition.X;
		if (shouldFaceRight != facingRight)
		{
			facingRight = shouldFaceRight;
			characterSprite.FlipH = !facingRight;
		}
	}

	private void OnBulletTimerTimeout()
	{
		if (Player == null) return;
		if (bulletScene == null)
		{
			GD.PrintErr("[Jereb] bulletScene is null — set PackedScene in inspector");
			return;
		}

		_bulletSpawner.SpawnBullet(Player, facingRight);
	}
	
	public override void ResetState()
	{
		base.ResetState();
		if (_bulletTimer != null)
		{
			_bulletTimer.Stop();
			_bulletTimer.WaitTime = bulletSpawnInterval;
		}
	}

	public override void Pause()
	{
		base.Pause();
		if (_bulletTimer != null && !_bulletTimer.IsStopped())
			_bulletTimer.Stop();
	}

	public override void Resume()
	{
		base.Resume();
		if (_distanceChecker != null && _distanceChecker.IsActive && _bulletTimer != null)
			_bulletTimer.Start();
	}
	
	public override void _ExitTree()
	{
		if (_bulletTimer != null)
			_bulletTimer.Timeout -= OnBulletTimerTimeout;

		base._ExitTree();
	}
}

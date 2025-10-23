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

		// Ініціалізація допоміжників
		_distanceChecker = new DistanceChecker(this, activationDistance);
		_bulletSpawner = new BulletSpawner(this, bulletScene, "enemy_bullet");

		// Timer для стрільби (вмикається/зупиняється в залежності від відстані)
		_bulletTimer = new Timer
		{
			OneShot = false,
			WaitTime = bulletSpawnInterval
		};
		AddChild(_bulletTimer);
		_bulletTimer.Timeout += OnBulletTimerTimeout;
		_bulletTimer.Stop(); // стартуємо лише коли гравець близько

		AddToGroup("enemy");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (isDead) return; // не працюємо якщо померли

		if (Player == null)
		{
			// Спробуємо знайти гравця (якщо ще не встановлено)
			Player = GetTree().GetFirstNodeInGroup("player") as BaseCharacter;
			if (Player == null) return;
		}

		// Перевірка дистанції викликається кожен кадр
		_distanceChecker.CheckDistance(Player.GlobalPosition);

		// Керуємо таймером в залежності від дистанції
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
		if (shouldFaceRight != FacingRight)
		{
			FacingRight = shouldFaceRight;
			// Якщо FacingRight == true -> FlipH = false (спрайт не фліпати)
			characterSprite.FlipH = !FacingRight;
		}
	}

	private void OnBulletTimerTimeout()
	{
		// перевірки
		if (Player == null) return;
		if (bulletScene == null)
		{
			GD.PrintErr("[Jereb] bulletScene is null — set PackedScene in inspector");
			return;
		}

		// Виклик спавнера
		_bulletSpawner.SpawnBullet(Player, FacingRight);
	}

	// Коли ворога повертають в пул, ResetState має відновити його і зупинити таймери
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

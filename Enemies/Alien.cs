/// <file>
/// <summary>
/// Alien.cs - Implements a specific enemy type (<c>Alien</c>) that uses timed shooting mechanics and activation based on player proximity.
/// </summary>
/// <remarks>
/// This enemy inherits from <c>BaseEnemy</c> and utilizes helper classes (<c>DistanceChecker</c>, <c>BulletSpawner</c>)
/// and a <see cref="Timer"/> to control its behavior: activating its attack pattern when the player is within a certain range
/// and periodically shooting at the player.
/// </remarks>
/// </file>
using Godot;
using System;

/// <summary>
/// A specific implementation of a ranged enemy. The <c>Alien</c> enemy remains passive until the player
/// enters its activation range, upon which it starts shooting bullets at a fixed interval.
/// </summary>
public partial class Alien : BaseEnemy
{
	/// <summary>
    /// The interval (in seconds) between bullet spawns once the enemy is active. Default is 3.0 seconds.
    /// </summary>
	private float bulletSpawnInterval = 3.0f;
	/// <summary>
    /// The <see cref="PackedScene"/> resource for the bullet this enemy fires. Must be loaded from "res://Enemies/Enemy_bullet.tscn".
    /// </summary>
    private PackedScene bulletScene;

    /// <summary>
    /// The maximum distance (in pixels) between the enemy and the player at which the enemy becomes active and starts shooting.
    /// </summary>
    private float activationDistance = 600f;

    /// <summary>
    /// Reference to the <see cref="AnimatedSprite2D"/> component used for enemy visuals and facing direction control.
    /// </summary>
    private AnimatedSprite2D characterSprite;

    /// <summary>
    /// The timer that controls the rate of bullet spawning.
    /// </summary>
    private Timer _bulletTimer;

    /// <summary>
    /// Helper object responsible for checking and tracking the distance to the target (player) and managing the activation state.
    /// Assumes <c>DistanceChecker</c> is a separate utility class.
    /// </summary>
    private DistanceChecker _distanceChecker;

    /// <summary>
    /// Helper object responsible for handling the actual creation and launching of bullets.
    /// Assumes <c>BulletSpawner</c> is a separate utility class.
    /// </summary>
    private BulletSpawner _bulletSpawner;

	/// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// Initializes components, loads resources, sets up the shooting timer, and subscribes to events.
    /// </summary>
	public override void _Ready()
	{
		base._Ready();

		// Node and Resource Retrieval
		characterSprite = GetNodeOrNull<AnimatedSprite2D>("EnemySprite");
		bulletScene = GD.Load<PackedScene>("res://Enemies/Enemy_bullet.tscn");
		if (characterSprite == null)
			GD.PrintErr("[Alien] Missing AnimatedSprite2D 'EnemySprite')");
		if (bulletScene == null)
			GD.PrintErr("[Alien] Missing PackedScene 'bulletScene'");

		// Helper Initialization
		_distanceChecker = new DistanceChecker(this, activationDistance);
		_bulletSpawner = new BulletSpawner(this, bulletScene, "enemy_bullet");

		// Timer Setup
		_bulletTimer = new Timer
		{
			OneShot = false,
			WaitTime = bulletSpawnInterval
		};
		
		//GD.Print($"[Alien] bulletSpawnInterval {bulletSpawnInterval}");
		
		AddChild(_bulletTimer);
		_bulletTimer.Timeout += OnBulletTimerTimeout;
		_bulletTimer.Stop(); // Start in inactive state

		AddToGroup("enemy");
	}

	/// <summary>
    /// Called every frame. Handles player tracking, distance checking, and activating/deactivating the firing timer.
    /// </summary>
    /// <param name="delta">The elapsed time since the previous frame.</param>
	public override void _Process(double delta)
	{
		base._Process(delta);

		if (isDead) return; 

		// Ensure Player reference is found
		if (Player == null)
		{
			Player = GetTree().GetFirstNodeInGroup("player") as BaseCharacter;
			if (Player == null) return;
		}
		// Distance Check and Activation Logic
		_distanceChecker.CheckDistance(Player.GlobalPosition);

		if (_distanceChecker.IsActive)
		{
			// Activate the timer if the enemy is within range and the timer is stopped
			if (_bulletTimer.IsStopped())
			{
				_bulletTimer.WaitTime = bulletSpawnInterval;
				_bulletTimer.Start();
			}
		}
		else
		{
			// Stop the timer if the enemy is outside range
			if (!_bulletTimer.IsStopped())
				_bulletTimer.Stop();
		}

		UpdateFacingTowardsPlayer();
	}

	/// <summary>
    /// Updates the enemy's visual direction (sprite flip) to face the player.
    /// </summary>
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

	/// <summary>
    /// Handler method called when the <see cref="_bulletTimer"/> times out.
    /// Triggers the <see cref="_bulletSpawner"/> to fire a bullet towards the player.
    /// </summary>
	private void OnBulletTimerTimeout()
	{
		if (Player == null) return;
		if (bulletScene == null)
		{
			GD.PrintErr("[Alien] bulletScene is null — set PackedScene in inspector");
			return;
		}
		_bulletSpawner.SpawnBullet(Player, facingRight);
	}
	
	/// <summary>
    /// Resets the enemy's state upon reuse (e.g., when returned from an object pool).
    /// Stops the firing timer and resets its interval.
    /// </summary>
	public override void ResetState()
	{
		base.ResetState();
		if (_bulletTimer != null)
		{
			_bulletTimer.Stop();
			_bulletTimer.WaitTime = bulletSpawnInterval;
		}
	}

	/// <summary>
    /// Pauses the enemy's processing and stops the firing timer if it was running.
    /// </summary>
	public override void Pause()
	{
		base.Pause();
		if (_bulletTimer != null && !_bulletTimer.IsStopped())
			_bulletTimer.Stop();
	}

	/// <summary>
    /// Resumes the enemy's processing and restarts the firing timer if the player is currently within the activation range.
    /// </summary>
	public override void Resume()
	{
		base.Resume();
		if (_distanceChecker != null && _distanceChecker.IsActive && _bulletTimer != null)
			_bulletTimer.Start();
	}
	
	/// <summary>
    /// Called when the node is about to exit the scene tree.
    /// Disconnects the timer's signal to prevent dangling references.
    /// </summary>
	public override void _ExitTree()
	{
		if (_bulletTimer != null)
			_bulletTimer.Timeout -= OnBulletTimerTimeout;

		base._ExitTree();
	}
}

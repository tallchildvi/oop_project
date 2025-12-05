/// <file>
/// <summary>
/// EnemyManager.cs - Manages the spawning, lifecycle, and pooling of enemies for waves.
/// </summary>
/// <remarks>
/// This manager orchestrates the waves of enemies using a <see cref="Timer"/> for controlled
/// spawning intervals and an <c>EnemyPool</c> for performance optimization via object pooling.
/// It tracks active enemies and triggers events like "WAVE_CLEARED" when a wave is complete.
/// </remarks>
/// </file>
using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Controls the spawning and management of enemy waves in the game.
/// It uses an object pooling system for efficient enemy creation and destruction.
/// </summary>
public partial class EnemyManager : Node
{
	/// <summary>
    /// Reference to the object pool responsible for providing and recycling enemy instances.
    /// </summary>
    private EnemyPool _enemyPool;

    /// <summary>
    /// A list tracking all currently active (spawned and alive) enemies in the scene.
    /// </summary>
    private List<BaseEnemy> _activeEnemies = new();

    /// <summary>
    /// The total number of enemies planned to be spawned for the current wave.
    /// </summary>
    private int _enemiesToSpawn = 0;

    /// <summary>
    /// The number of enemies that have been spawned so far in the current wave.
    /// </summary>
    private int _spawnedEnemies = 0;

    /// <summary>
    /// The time interval (in seconds) between spawning two consecutive enemies. Defaults to 0.8 seconds.
    /// </summary>
    private float _spawnInterval = 0.8f;

    /// <summary>
    /// Flag indicating whether a wave is currently in progress (i.e., enemies are being spawned or are active).
    /// </summary>
    private bool _waveInProgress = false;

    /// <summary>
    /// Flag indicating whether the enemy manager (and active enemies) are currently paused.
    /// </summary>
    private bool _isPaused = false;

    /// <summary>
    /// The timer responsible for controlling the enemy spawn rate.
    /// </summary>
    private Timer _spawnTimer;

	/// <summary>
    /// Initializes the EnemyManager by setting up the object pool and the spawn timer.
    /// This method must be called before <see cref="StartWave"/>.
    /// </summary>
    /// <param name="enemyPool">The <c>EnemyPool</c> instance to use for enemy creation.</param>
    /// <param name="spawnInterval">The time (in seconds) between enemy spawns. Defaults to 0.8f.</param>
	public void Initialize(EnemyPool enemyPool, float spawnInterval = 0.8f)
	{
		_enemyPool = enemyPool;
		_spawnInterval = spawnInterval;

		_spawnTimer = new Timer
		{
			OneShot = false,
			WaitTime = _spawnInterval
		};
		AddChild(_spawnTimer);
		_spawnTimer.Timeout += OnSpawnTimerTimeout;
	}

	/// <summary>
    /// Starts a new wave of enemies, initializing spawn counters and starting the spawn timer.
    /// </summary>
    /// <param name="enemyCount">The total number of enemies to spawn in this wave.</param>
	public void StartWave(int enemyCount)
	{
		if (_waveInProgress)
		{
			GD.PrintErr("Cannot start new wave while previous is active!");
			return;
		}

		if (_enemyPool == null)
		{
			GD.PrintErr("[EnemyManager] EnemyPool is null, cannot start wave");
			return;
		}

		_waveInProgress = true;
		_enemiesToSpawn = enemyCount;
		_spawnedEnemies = 0;
		_activeEnemies.Clear();

		GD.Print($"[EnemyManager] Starting new wave: {enemyCount} enemies");
		_spawnTimer.Start();
	}

	/// <summary>
    /// Handler method called every time the spawn timer times out.
    /// Spawns one enemy from the pool, places it at a random position, and tracks it.
    /// Stops the timer once all planned enemies are spawned.
    /// </summary>
	private void OnSpawnTimerTimeout()
	{
		if (_isPaused) return;

		if (_spawnedEnemies >= _enemiesToSpawn)
		{
			_spawnTimer.Stop();
			GD.Print("[EnemyManager] All enemies spawned for this wave.");
			return;
		}

		GD.Print($"[EnemyManager] Spawning enemy {_spawnedEnemies + 1}/{_enemiesToSpawn}");

		var enemy = _enemyPool.GetEnemy();
		if (enemy == null)
		{
			GD.PrintErr("[EnemyManager] Could not spawn enemy (pool empty).");
			return;
		}

		Vector2 spawnPos = GetRandomSpawnPosition();
		GD.Print($"[EnemyManager] Setting position: {spawnPos}");
		
		// Setup and activate the enemy
		enemy.Position = spawnPos;
		enemy.Visible = true;
		enemy.ResetState();
		// Track the enemy and subscribe to its death event
		_activeEnemies.Add(enemy);
		enemy.Died += OnEnemyDied;

		_spawnedEnemies++;
		GD.Print($"[EnemyManager] Spawned successfully! Active: {_activeEnemies.Count}");
	}

	//private Vector2 GetRandomSpawnPosition()
	//{
		//var windowSize = DisplayServer.WindowGetSize();
		//var rand = new RandomNumberGenerator();
		//rand.Randomize();
		//bool aboveScene = rand.Randf() < 0.5f;
		//bool fromLeft = rand.Randf() < 0.5f;
		//int randX = fromLeft ? (int)rand.RandfRange(-200, -100) : (int)rand.RandfRange(windowSize.X + 100, windowSize.X + 200);
		//int randY = aboveScene ? (int)rand.RandfRange(-200, -100) : (int)rand.RandfRange(windowSize.Y + 100, windowSize.Y + 200);
		//var position = new Vector2(randX, randY);
		//GD.Print($"[EnemyManager] random position for enemy: {position}");
		//return position;
	//}

	/// <summary>
    /// Calculates a random spawn position outside the visible screen area to simulate enemies entering the scene.
    /// The position is determined by randomly selecting one of the four sides (top, right, bottom, left).
    /// </summary>
    /// <returns>A <see cref="Vector2"/> representing a position safely off-screen.</returns>
	private Vector2 GetRandomSpawnPosition()
	{
		var windowSize = DisplayServer.WindowGetSize();
		var rand = new RandomNumberGenerator();
		rand.Randomize();

		int side = rand.RandiRange(0, 3);
		Vector2 pos = Vector2.Zero;

		switch (side)
		{
			case 0: // Top
				pos.X = rand.RandfRange(-100, windowSize.X + 100);
				pos.Y = rand.RandfRange(-200, -100);
				break;

			case 1: // Right
				pos.X = rand.RandfRange(windowSize.X + 100, windowSize.X + 200);
				pos.Y = rand.RandfRange(-100, windowSize.Y + 100);
				break;

			case 2: // Bottom
				pos.X = rand.RandfRange(-100, windowSize.X + 100);
				pos.Y = rand.RandfRange(windowSize.Y + 100, windowSize.Y + 200);
				break;

			case 3: // Left
				pos.X = rand.RandfRange(-200, -100);
				pos.Y = rand.RandfRange(-100, windowSize.Y + 100);
				break;
		}

		GD.Print($"[EnemyManager] Spawn position for enemy: {pos}");
		return pos;
	}

	/// <summary>
    /// Handler method called when an enemy dies (via the <c>Died</c> event).
    /// Cleans up the dead enemy, returns it to the pool, and checks for wave completion.
    /// </summary>
    /// <param name="enemy">The <c>BaseEnemy</c> instance that died.</param>
	private void OnEnemyDied(BaseEnemy enemy)
	{
		if (enemy == null) return;
		enemy.RemoveFromGroup("enemy");

		// Unsubscribe from the event to prevent memory leaks or calling on pooled enemies
		enemy.Died -= OnEnemyDied;

		if (!_activeEnemies.Contains(enemy)) return;

		// Clean up tracking and pooling
		_activeEnemies.Remove(enemy);
		_enemyPool.ReturnEnemy(enemy);

		// Notify other systems of the kill
		EventManager.TriggerEvent("ENEMY_KILLED", enemy);

		// Check for wave end condition
		if (IsCleared())
		{
			GD.Print("[EnemyManager] Wave cleared!");
			_waveInProgress = false;
			EventManager.TriggerEvent("WAVE_CLEARED", this);
		}
	}

	/// <summary>
    /// Checks if the current enemy wave has been cleared.
    /// </summary>
    /// <returns>
    /// <c>true</c> if no wave is in progress, OR if all enemies have been spawned AND no enemies are currently active; otherwise, <c>false</c>.
    /// </returns>
	public bool IsCleared() =>
		!_waveInProgress || (_activeEnemies.Count == 0 && _spawnedEnemies >= _enemiesToSpawn);

	/// <summary>
    /// Pauses the wave progression by stopping the spawn timer and pausing all active enemies.
    /// </summary>
	public void Pause()
	{
		_isPaused = true;
		foreach (var e in _activeEnemies)
			e.Pause();
		_spawnTimer.Stop();
	}

	/// <summary>
    /// Resumes the wave progression by resuming all active enemies and restarting the spawn timer if a wave is ongoing.
    /// </summary>
	public void Resume()
	{
		_isPaused = false;
		foreach (var e in _activeEnemies)
			e.Resume();
		if (_waveInProgress)
			_spawnTimer.Start();
	}
	
	/// <summary>
    /// Immediately clears all active enemies, stops the wave, and resets the manager state.
    /// All active enemies are returned to the pool.
    /// </summary>
	public void Clear()
	{
		foreach (var e in _activeEnemies)
		{
			// Clean up subscriptions before returning to pool
			e.Died -= OnEnemyDied;
			_enemyPool.ReturnEnemy(e);
		}
		_activeEnemies.Clear();
		_spawnTimer.Stop();
		_waveInProgress = false;
		_enemiesToSpawn = 0;
		_spawnedEnemies = 0;
	}
}

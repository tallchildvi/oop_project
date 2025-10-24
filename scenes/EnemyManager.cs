using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyManager : Node
{
	private EnemyPool _enemyPool;
	private List<BaseEnemy> _activeEnemies = new();
	private int _enemiesToSpawn = 0;
	private int _spawnedEnemies = 0;

	private float _spawnInterval = 0.8f;
	private bool _waveInProgress = false;
	private bool _isPaused = false;

	private Timer _spawnTimer;

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

	private void OnSpawnTimerTimeout()
	{
		if (_isPaused) return;

		if (_spawnedEnemies >= _enemiesToSpawn)
		{
			_spawnTimer.Stop();
			GD.Print("[EnemyManager] All enemies spawned for this wave.");
			return;
		}

		var enemy = _enemyPool.GetEnemy();
		if (enemy == null)
		{
			GD.PrintErr("[EnemyManager] Could not spawn enemy (pool empty).");
			return;
		}

		enemy.Position = GetRandomSpawnPosition();
		enemy.Visible = true;
		enemy.ResetState();
		_activeEnemies.Add(enemy);
		enemy.Died += OnEnemyDied;

		_spawnedEnemies++;
	}

	private Vector2 GetRandomSpawnPosition()
	{
		var windowSize = DisplayServer.WindowGetSize();
		var rand = new RandomNumberGenerator();
		rand.Randomize();
		bool aboveScene = rand.Randf() < 0.5f;
		bool fromLeft = rand.Randf() < 0.5f;
		int randX = fromLeft ? (int)rand.RandfRange(-200, -100) : (int)rand.RandfRange(windowSize.X + 100, windowSize.X + 200);
		int randY = aboveScene ? (int)rand.RandfRange(-200, -100) : (int)rand.RandfRange(windowSize.Y + 100, windowSize.Y + 200);
		var position = new Vector2(randX, randY);
		GD.Print($"[EnemyManager] random position for enemy: {position}");
		return position;
	}

	private void OnEnemyDied(BaseEnemy enemy)
	{
		if (enemy == null) return;

		// Відписуємось від події
		enemy.Died -= OnEnemyDied;

		if (!_activeEnemies.Contains(enemy)) return;

		_activeEnemies.Remove(enemy);
		_enemyPool.ReturnEnemy(enemy);

		EventManager.TriggerEvent("ENEMY_KILLED", enemy);

		if (IsCleared())
		{
			GD.Print("[EnemyManager] Wave cleared!");
			_waveInProgress = false;
			EventManager.TriggerEvent("WAVE_CLEARED", this);
		}
	}

	public bool IsCleared() =>
		!_waveInProgress || (_activeEnemies.Count == 0 && _spawnedEnemies >= _enemiesToSpawn);

	public void Pause()
	{
		_isPaused = true;
		foreach (var e in _activeEnemies)
			e.Pause();
		_spawnTimer.Stop();
	}

	public void Resume()
	{
		_isPaused = false;
		foreach (var e in _activeEnemies)
			e.Resume();
		if (_waveInProgress)
			_spawnTimer.Start();
	}

	public void Clear()
	{
		foreach (var e in _activeEnemies)
		{
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

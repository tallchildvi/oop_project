using Godot;
using System;
using System.Collections.Generic;

public partial class EnemyManager : Node
{
	private EnemyPool _enemyPool;

	private List<Enemy> _activeEnemies = new();
	private int _enemiesToSpawn = 0;
	private int _spawnedEnemies = 0;

	private float _spawnInterval = 0.8f;
	private bool _waveInProgress = false;
	private bool _isPaused = false;

	private Timer _spawnTimer;

	public void Initialize(EnemyPool enemyPool)
	{
		_enemyPool = enemyPool;
		_spawnTimer = new Timer
		{
			OneShot = false,
			WaitTime = _spawnInterval
		};
		AddChild(_spawnTimer);
		_spawnTimer.Timeout += OnSpawnTimerTimeout;
	}

	// === 2️⃣ СТАРТ ХВИЛІ ===
	public void StartWave(int enemyCount)
	{
		if (_waveInProgress)
		{
			GD.PrintErr("Cannot start new wave while previous is active!");
			return;
		}

		_waveInProgress = true;
		_enemiesToSpawn = enemyCount;
		_spawnedEnemies = 0;
		_activeEnemies.Clear();

		GD.Print($"[EnemyManager] Starting new wave: {enemyCount} enemies");

		_spawnTimer.Start();
	}

	// === 3️⃣ СПАВН ===
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
			GD.PrintErr("[EnemyManager] Pool empty! Could not spawn enemy.");
			return;
		}

		// Встановлення позиції (можна вдосконалити)
		enemy.Position = GetRandomSpawnPosition();

		AddChild(enemy);
		_activeEnemies.Add(enemy);

		// Підписка на подію смерті
		enemy.Died += OnEnemyDied;

		_spawnedEnemies++;
	}

	private Vector2 GetRandomSpawnPosition()
	{
		var rand = new RandomNumberGenerator();
		return new Vector2(rand.RandfRange(-300, 300), rand.RandfRange(-200, 200));
	}

	// === 4️⃣ СМЕРТЬ ВОРОГА ===
	private void OnEnemyDied(Enemy enemy)
	{
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

	public bool IsCleared()
	{
		return !_waveInProgress || (_activeEnemies.Count == 0 && _spawnedEnemies >= _enemiesToSpawn);
	}

	public void Pause()
	{
		_isPaused = true;
		foreach (var e in _activeEnemies)
			e.Pause();
	}

	public void Resume()
	{
		_isPaused = false;
		foreach (var e in _activeEnemies)
			e.Resume();
	}

	public void ClearAllEnemies()
	{
		foreach (var e in _activeEnemies)
		{
			if (IsInstanceValid(e))
			{
				e.QueueFree();
			}
		}
		_activeEnemies.Clear();
	}

	public override void _ExitTree()
	{
		_spawnTimer?.Stop();
		ClearAllEnemies();
	}
}

using Godot;
using System;

public class Level : Node
{
	public string MapName { get; set; }
	public Player Player { get; set; }
	public UIManager UI { get; set; }
	public EnemyManager EnemyManager { get; set; }
	public EnemyPool EnemyPool { get; set; }
	private bool _isActive = false;

	private int _currentWave = 0;
	private int _maxWaves = 5;
	private int _baseEnemies = 3;
	private float _difficultyMultiplier = 1.25f;

	public override void _Ready()
	{
		EventManager.Subscribe("PAUSE_GAME", OnPause);
		EventManager.Subscribe("RESUME_GAME", OnResume);
		EventManager.Subscribe("ENEMY_KILLED", OnEnemyKilled);
		EventManager.Subscribe("GAME_OVER", OnPlayerDied);
	}

	public void Start()
	{
		_isActive = true;
		_eventManager.Emit("OnLevelStarted", this);
//change to event system
		UI.Show();
		Player.EnableControl(true);

		GD.Print("Starting wave system...");
		StartNextWave();
	}

	private void StartNextWave()
	{
		//if (_currentWave >= _maxWaves)
		//{
			//GD.Print("All waves completed!");
			//_eventManager.Emit("OnLevelCleared", this);
			//return;
		//}

		_currentWave++;
		GD.Print($"=== Starting Wave {_currentWave} ===");

		int enemyCount = CalculateEnemyCount(_currentWave);
		EnemyManager.SpawnWave(enemyCount, EnemyPool);

		//UI.UpdateWaveCounter(_currentWave, _maxWaves);
	}

	private int CalculateEnemyCount(int wave)
	{
		return Mathf.RoundToInt(_baseEnemies * Mathf.Pow(_difficultyMultiplier, wave - 1));
	}

	private void OnEnemyKilled(object enemy)
	{
		if (enemy is Enemy e)
		{
			EnemyPool.ReturnEnemy(e);
		}

		if (EnemyManager.IsCleared())
		{
			GD.Print("Wave cleared!");
			_eventManager.Emit("OnWaveCleared", _currentWave);

			GetTree().CreateTimer(2.0f).Timeout += StartNextWave;
		}
	}

	private void OnPlayerDied(object data)
	{
		GD.Print("Player died");
		EvenManager.TriggerEvent("OnLevelFailed", this);
	}

	private void OnPause(object data)
	{
		if (!_isActive) return;
		_isActive = false;
		Player.EnableControl(false);
		EnemyManager.Pause();
		UI.ShowPauseMenu();
	}

	private void OnResume(object data)
	{
		_isActive = true;
		Player.EnableControl(true);
		EnemyManager.Resume();
		UI.HidePauseMenu();
	}

	public void End()
	{
		GD.Print("Level ended");
		_isActive = false;
		_eventManager.Emit("OnLevelEnded", this);
		_eventManager.UnsubscribeAll(this);
		QueueFree();
	}
}

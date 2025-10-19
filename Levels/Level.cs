using Godot;
using System;

public partial class Level : Node
{
	public string MapName { get; set; }
	public BaseCharacter Player { get; set; }
	public LevelUI UI { get; set; }
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
		GD.Print("=== Level.Start() ===");
		GD.Print($"UI == null? {UI == null}");
		GD.Print($"Player == null? {Player == null}");
		GD.Print($"EnemyManager == null? {EnemyManager == null}");
		_isActive = true;
		EventManager.TriggerEvent("LEVEL_STARTED", this);
//change to event system
		//UI.Show();
		Player.EnableControl();

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
		EnemyManager.StartWave(enemyCount);

		//UI.UpdateWaveCounter(_currentWave, _maxWaves);
	}

	private int CalculateEnemyCount(int wave)
	{
		return Mathf.RoundToInt(_baseEnemies * Mathf.Pow(_difficultyMultiplier, wave - 1));
	}

	private void OnEnemyKilled(object enemy)
	{
		if (enemy is BaseEnemy e)
		{
			EnemyPool.ReturnEnemy(e);
		}

		if (EnemyManager.IsCleared())
		{
			GD.Print("Wave cleared!");
			EventManager.TriggerEvent("OnWaveCleared", _currentWave);

			GetTree().CreateTimer(2.0f).Timeout += StartNextWave;
		}
	}

	private void OnPlayerDied(object data)
	{
		GD.Print("Player died");
		EventManager.TriggerEvent("OnLevelFailed", this);
	}

	private void OnPause(object data)
	{
		if (!_isActive) return;
		_isActive = false;
		Player.DisableControl();
		EnemyManager.Pause();
		EventManager.TriggerEvent("OPEN_MENU", "PauseMenu");
	}

	private void OnResume(object data)
	{
		_isActive = true;
		Player.EnableControl();
		EnemyManager.Resume();
		EventManager.TriggerEvent("CLOSE_MENU", "PauseMenu");
	}

	public void End()
	{
		GD.Print("Level ended");
		_isActive = false;
		EventManager.TriggerEvent("OnLevelEnded", this);
		QueueFree();
	}
}

using Godot;
using System;

public class Level : Node
{
	private PackedScene _sceneResource;
	private Node _sceneInstance;
	private EnemyManager _enemyManager;
	private Player _player;
	private UIManager _uiManager;
	private EventManager _eventManager;

	private bool _isActive = false;

	public Level(PackedScene sceneResource, EnemyManager enemyManager, Player player, UIManager uiManager, EventManager eventManager)
	{
		_sceneResource = sceneResource;
		_enemyManager = enemyManager;
		_player = player;
		_uiManager = uiManager;
		_eventManager = eventManager;
	}
	
	public void Initialize()
	{
		_eventManager.Subscribe("PauseGame", OnPause);
		_eventManager.Subscribe("ResumeGame", OnResume);
		_eventManager.Subscribe("EnemyKilled", OnEnemyKilled);
		_eventManager.Subscribe("PlayerDied", OnPlayerDied);

		_sceneInstance = _sceneResource.Instantiate();
		AddChild(_sceneInstance);

		_sceneInstance.AddChild(_enemyManager);
		_sceneInstance.AddChild(_player);
		_sceneInstance.AddChild(_uiManager);
	}

	public void Start()
	{

		_isActive = true;

		_eventManager.Emit("OnLevelStarted", this);

		_enemyManager.Activate();
		_uiManager.Show();
		_player.EnableControl(true);
	}

	private void OnPause(object data)
	{
		if (!_isActive) return;

		_isActive = false;
		_player.EnableControl(false);
		_enemyManager.Pause();
		_uiManager.ShowPauseMenu();

		GD.Print("Level paused");
	}

	private void OnResume(object data)
	{
		_isActive = true;
		_player.EnableControl(true);
		_enemyManager.Resume();
		_uiManager.HidePauseMenu();

		GD.Print("Level resumed");
	}

	// 💀 4. Завершення
	public void End()
	{
		GD.Print("Level ended");
		_isActive = false;

		_eventManager.Emit("OnLevelEnded", this);
		_eventManager.UnsubscribeAll(this);

		QueueFree();
	}

	// 🧩 5. Реакція на події
	private void OnEnemyKilled(object enemy)
	{
		GD.Print($"Enemy killed: {enemy}");

		if (_enemyManager.IsCleared())
		{
			_eventManager.Emit("OnLevelCompleted", this);
		}
	}

	private void OnPlayerDied(object data)
	{
		GD.Print("Player died");
		_eventManager.Emit("OnLevelFailed", this);
	}
}

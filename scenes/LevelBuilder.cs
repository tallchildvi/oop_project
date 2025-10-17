using Godot;
using System;

public partial class LevelBuilder : Node, ILevelBuilder
{
	private Level _currentLevel;

	public override void _Ready()
	{
		EventManager.Subscribe("CANCEL_GAME_START", CancelBuildingLevel);
	}

	public void StartBuilding()
	{
		var levelScene = GD.Load<PackedScene>("res://Levels/Level1.tscn");
		_currentLevel = levelScene.Instantiate<Level>();
		// Не додаємо у сцену тут — це зробить GameManager
	}

	public void CancelBuildingLevel(object _) => _currentLevel = null;

	public void BuildMap(string mapName)
	{
		var scenePath = $"res://Levels/{mapName}.tscn";
		var mapScene = GD.Load<PackedScene>(scenePath);
		var mapInstance = mapScene.Instantiate<Node>();

		_currentLevel.MapName = mapName;
		_currentLevel.AddChild(mapInstance);
	}

	public void BuildPlayer(string characterId)
	{
		var playerScene = GD.Load<PackedScene>($"res://Characters/{characterId}.tscn");
		var player = playerScene.Instantiate<Player>();
		_currentLevel.Player = player;
		_currentLevel.AddChild(player);
	}

	public void BuildEnemies(int baseDifficulty = 1)
	{
		var enemyScene = GD.Load<PackedScene>("res://Scenes/Enemy.tscn");

		var enemyPool = new EnemyPool();
		enemyPool.Initialize(enemyScene, _currentLevel, baseDifficulty * 3);

		var enemyManager = new EnemyManager();
		enemyManager.Initialize(enemyPool);

		// Тільки після Initialize() додаємо в сцену — тоді _Ready() виконається з коректними полями
		_currentLevel.AddChild(enemyPool);
		_currentLevel.AddChild(enemyManager);

		_currentLevel.EnemyPool = enemyPool;
		_currentLevel.EnemyManager = enemyManager;
	}

	public void BuildUI()
	{
		// TODO: додати UI побудову
	}

	public Level GetResult()
	{
		GD.Print("Level fully built and ready!");
		return _currentLevel;
	}

	public override void _ExitTree()
	{
		EventManager.Unsubscribe("CANCEL_GAME_START", CancelBuildingLevel);
	}
}

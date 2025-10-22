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
	}

	public void CancelBuildingLevel(object _) => _currentLevel = null;

	public void BuildMap(string mapName)
	{
		var scenePath = $"res://Levels/{mapName}.tscn";
		var mapScene = GD.Load<PackedScene>(scenePath);
		var mapInstance = mapScene.Instantiate<Node>();

		_currentLevel.MapName = mapName;
		//_currentLevel.AddChild(mapInstance);
	}

	public void BuildPlayer(string characterId)
	{
		var playerScene = GD.Load<PackedScene>($"res://Characters/{characterId}/{characterId}.tscn");
		if (playerScene == null) GD.Print("player scene is null");
		else GD.Print("player scene has found");
		var player = playerScene.Instantiate<BaseCharacter>();
		if (player == null) GD.Print("player is null");
		else GD.Print("player has found");
		_currentLevel.Player = player;
		GD.Print($"{_currentLevel.Player.speed}");
		_currentLevel.AddChild(player);

	}

	public void BuildEnemies(int baseDifficulty = 1)
	{
		var enemyScene = GD.Load<PackedScene>("res://Enemies/Enemy1.tscn");

		var enemyPool = new EnemyPool();
		enemyPool.Initialize(enemyScene, _currentLevel, baseDifficulty * 3);

		var enemyManager = new EnemyManager();
		enemyManager.Initialize(enemyPool);

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

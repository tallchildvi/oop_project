using Godot;
using System;

public partial class LevelBuilder : Node, ILevelBuilder
{
	private Level _currentLevel; 

	public override void _Ready(){
		EventManager.Subscribe("START_GAME", BuildNewLevel);
		EventManager.Subscribe("CANCEL_GAME_START", CancelBuildingLevel);
	}
	public void BuildNewLevel(object _){
		_currentLevel = new Level();
	}
	public void CancelBuildingLevel(object _){
		_currentLevel = null;
	}

	public void BuildMap(string mapName)
	{
		GD.Print($"Loading map: {mapName}");
		var mapScene = GD.Load<PackedScene>($"res://Levels/{mapName}.tscn");
		var mapInstance = mapScene.Instantiate<Node>();

		_currentLevel.MapName = mapName;
		_currentLevel.AddChild(mapInstance);
	}

	public void BuildPlayer(string characterId)
	{
		GD.Print($"Spawning player: {characterId}");
		var playerScene = GD.Load<PackedScene>($"res://Characters/{characterId}.tscn");
		var player = playerScene.Instantiate<Player>();

		_currentLevel.Player = player;
		_currentLevel.AddChild(player);
	}

	public void BuildEnemies(int baseDifficulty = 1)
	{
		GD.Print($"Setting up enemy pool with base difficulty {baseDifficulty}");
   		var enemyScene = GD.Load<PackedScene>("res://Scenes/Enemy.tscn");
   		var enemyPool = new EnemyPool(enemyScene, baseDifficulty * 3);
   		var enemyManager = new EnemyManager();
		enemyManager.Initialize(enemyPool);
		_currentLevel.EnemyPool = enemyPool;
   		_currentLevel.EnemyManager = enemyManager;
		_currentLevel.AddChild(enemyPool);
		_currentLevel.AddChild(enemyManager);
	}

	public void BuildUI()
	{
			//something
	}

		public Level GetResult()
		{
			GD.Print("Level fully built and ready!");
			return _currentLevel;
		}
	}

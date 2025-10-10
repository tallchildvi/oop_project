using Godot;
using System;

public partial class LevelBuilder : Node, ILevelBuilder
{
	private Level _currentLevel;

	public void BuildMap(string mapName)
	{
		GD.Print($"Loading map: {mapName}");
		_currentLevel = new Level();
		_currentLevel.MapName = mapName;
	}

	public void BuildPlayer(string characterId)
	{
		GD.Print($"Spawning player: {characterId}");
		_currentLevel.Player = new Player(characterId);
	}

	public void BuildEnemies(int difficulty)
	{
		GD.Print($"Spawning enemies with difficulty {difficulty}");
		_currentLevel.Enemies = EnemyFactory.CreateEnemies(difficulty);
	}

	public void BuildUI()
	{
		GD.Print("Loading HUD");
		_currentLevel.UI = new GameUI();
	}

	public Level GetResult()
	{
		return _currentLevel;
	}
}

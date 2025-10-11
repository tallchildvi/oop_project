using Godot;
using System;

public partial class GameManager : Node
{
	public override void _Ready()
	{
		GD.Print("GameLoader: initializing...");
		EventManager.Subscribe("START_GAME", StartGame);
	}
	public void StartGame()
	{
		var builder = new LevelBuilder();
		builder.BuildMap("Level1");
		builder.BuildPlayer("main_character");
		builder.BuildEnemies(3);
		builder.BuildUI();
		var level = builder.GetResult();
		AddChild(level);
		level.Start();
	}
}

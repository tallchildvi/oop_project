using Godot;
using System;

public partial class GameManager : Node
{
	public override void _Ready()
	{
		GD.Print("GameLoader: initializing...");
		EventManager.Subscribe("START_GAME", StartGame);
	}
	public void StartGame(object _)
	{
		GD.Print("GameManager: StartGame triggered!");
		var builder = new LevelBuilder();
		builder.StartBuilding();
		AddChild(builder);
		builder.BuildMap("Level1");
		builder.BuildPlayer("Atlas");
		builder.BuildEnemies(3);
		builder.BuildUI();
		var level = builder.GetResult();
		AddChild(level);
		level.Start();
	}
}

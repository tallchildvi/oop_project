using Godot;
using System;

public partial class GameManager : Node
{
	private LevelBuilder _builder;

	public override void _Ready()
	{
		GD.Print("[GameManager] initializing...");
		GD.Print($"[GameManager] screen size: {DisplayServer.ScreenGetSize(DisplayServer.WindowGetCurrentScreen())}");
		GD.Print($"[GameManager] window size: {DisplayServer.WindowGetSize()}");
		EventManager.Subscribe("START_GAME", StartGame);
		
	}

	public override void _ExitTree()
	{
		EventManager.Unsubscribe("START_GAME", StartGame);
	}

	public void StartGame(object _)
	{
		GD.Print("[GameManager] StartGame triggered!");
		_builder = new LevelBuilder();
		_builder.StartBuilding();
		AddChild(_builder);

		_builder.BuildMap("Level1");
		_builder.BuildPlayer("Atlas");
		_builder.BuildEnemies(3);
		_builder.BuildUI();

		var level = _builder.GetResult();
		if (level == null)
		{
			GD.PrintErr("[GameManager] LevelBuilder returned null level");
			return;
		}

		AddChild(level);
		level.Start();

		_builder.QueueFree();
		_builder = null;
	}
}

/// <file>
/// <summary>
/// GameManager.cs - The core manager class responsible for initializing and controlling the game state and flow.
/// </summary>
/// <remarks>
/// The <see cref="GameManager"/> handles key lifecycle events, such as subscribing to the START_GAME
/// event and orchestrating the creation of a new level using the <c>LevelBuilder</c> pattern.
/// It also logs important system information upon initialization.
/// </remarks>
/// </file>
using Godot;
using System;

/// <summary>
/// The central manager responsible for controlling the overall game state, level flow, and initialization.
/// It uses the <see cref="EventManager"/> to initiate the game start sequence.
/// </summary>
public partial class GameManager : Node
{
	/// <summary>
    /// A private reference to the <c>LevelBuilder</c> instance, used to construct the game world elements.
    /// This variable is set and cleared during the game start process.
    /// </summary>
	private LevelBuilder _builder;

	/// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// Initializes the manager, logs screen information, and subscribes to the "START_GAME" event.
    /// </summary>
	public override void _Ready()
	{
		GD.Print("[GameManager] initializing...");
		// Log display and window size information for debugging purposes
		GD.Print($"[GameManager] screen size: {DisplayServer.ScreenGetSize(DisplayServer.WindowGetCurrentScreen())}");
		GD.Print($"[GameManager] window size: {DisplayServer.WindowGetSize()}");
		// Subscribe to the event that triggers the game start sequence.
		EventManager.Subscribe("START_GAME", StartGame);
		
	}

	/// <summary>
    /// Called when the node is about to exit the scene tree.
    /// Unsubscribes from the "START_GAME" event to prevent errors.
    /// </summary>
	public override void _ExitTree()
	{
		EventManager.Unsubscribe("START_GAME", StartGame);
	}

	/// <summary>
    /// Initiates the game start process. This method is called when the "START_GAME" event is triggered.
    /// It utilizes the <c>LevelBuilder</c> pattern to construct the game environment, player, and UI.
    /// </summary>
    /// <param name="_">The event parameter (ignored).</param>
	public void StartGame(object _)
	{
		GD.Print("[GameManager] StartGame triggered!");

		// 1. Initialize the Level Builder
		_builder = new LevelBuilder();
		_builder.StartBuilding();
		AddChild(_builder);// Add the builder to the scene tree (if needed for internal logic/deferred calls)

        // 2. Direct the builder to construct various game components
		_builder.BuildMap("Level1");
		_builder.BuildPlayer("Atlas");
		_builder.BuildEnemies();
		_builder.BuildUI();

		// 3. Get the final level object (the result of the construction)
		var level = _builder.GetResult();
		if (level == null)
		{
			GD.PrintErr("[GameManager] LevelBuilder returned null level");
			return;
		}

		// 4. Add the completed level to the scene tree and start it
		AddChild(level);
		// Assumes the level object has a Start() method
		level.Start();

		// 5. Clean up the Level Builder
		_builder.QueueFree();
		_builder = null;
	}

}

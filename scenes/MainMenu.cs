/// <file>
/// <summary>
/// MainMenu.cs - Controls the logic and initialization of the main menu screen.
/// </summary>
/// <remarks>
/// This script handles the initialization of UI elements (<see cref="TextureButton"/>s)
/// and sets up event subscriptions to control the flow of the game, such as starting
/// the game and opening the settings menu, primarily using the <see cref="EventManager"/>.
/// </remarks>
/// </file>
using Godot;
using System;

/// <summary>
/// Manages the behavior of the main menu interface, handling button interactions and initialization.
/// It inherits from <see cref="Control"/> as it represents a primary UI screen.
/// </summary>
public partial class MainMenu : Control{
	
	/// <summary>
    /// Reference to the button used to start the game.
    /// </summary>
	private TextureButton _playButton;
	/// <summary>
    /// Reference to the button used to open the game settings popup.
    /// </summary>
	private TextureButton _settingsButton;
	/// <summary>
    /// Reference to the settings panel/popup node.
    /// </summary>
	private Panel _settingsPopup;
	
	/// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// This method is asynchronous to wait for the next frame before triggering events.
    /// </summary>
	public async  override void _Ready(){
		// Debugging block: Finds and prints information about nodes in the "player" group.
		var players = GetTree().GetNodesInGroup("player");
		GD.Print($"[DEBUG] Players in tree: {players.Count}");
		foreach (Node p in players)
		{
			GD.Print($"[DEBUG] - name: {p.Name}, id: {p.GetInstanceId()}, path: {p.GetPath()}, parent: {p.GetParent()?.Name}");
		}
		// Waits for the next physics frame to ensure all other components have been initialized
        // before interacting with the EventManager.
		await ToSignal(GetTree(), "process_frame");
		// Triggers a global event to close any menus that might be open from previous scenes/sessions.
   		EventManager.TriggerEvent("CLOSE_ALL_MENUS");
		// --- Node Retrieval ---
		_playButton = GetNode<TextureButton>("MarginContainer/VBoxContainer/HBoxContainer/play_button");
		_settingsButton = GetNode<TextureButton>("MarginContainer/VBoxContainer/HBoxContainer/settings_button");
		_settingsPopup = GetNode<Panel>("SettingsMenu");
		
		// --- Play Button Initialization and Configuration ---
		_playButton.TextureNormal = GD.Load<Texture2D>("res://source/button_play.png");
		_playButton.Size = new Vector2(100, 100);
		_playButton.StretchMode = TextureButton.StretchModeEnum.KeepAspect;
		
		// --- Settings Button Initialization and Configuration ---
		_settingsButton.TextureNormal = GD.Load<Texture2D>("res://source/button_settings.png");
		_settingsButton.CustomMinimumSize = new Vector2(100, 100);
		_settingsButton.StretchMode = TextureButton.StretchModeEnum.KeepAspect;
		
		// --- Event Subscription/Button Binding ---
        /// <summary>
        /// Binds a lambda function to the <see cref="TextureButton.Pressed"/> signal to trigger the "OPEN_MENU" event
        /// with the parameter "SettingsMenu".
        /// </summary>
		_settingsButton.Pressed += () => EventManager.TriggerEvent("OPEN_MENU", "SettingsMenu");
		
		/// <summary>
        /// Binds a lambda function to the <see cref="TextureButton.Pressed"/> signal to trigger the "START_GAME" event.
        /// </summary>
		_playButton.Pressed += () => EventManager.TriggerEvent("START_GAME");
		
	}
}

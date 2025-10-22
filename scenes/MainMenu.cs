using Godot;
using System;

public partial class MainMenu : Control{
	
	private TextureButton _playButton;
	private TextureButton _settingsButton;
	private Panel _settingsPopup;
	
	public async  override void _Ready(){
		var players = GetTree().GetNodesInGroup("player");
		GD.Print($"[DEBUG] Players in tree: {players.Count}");
		foreach (Node p in players)
		{
			GD.Print($"[DEBUG] - name: {p.Name}, id: {p.GetInstanceId()}, path: {p.GetPath()}, parent: {p.GetParent()?.Name}");
		}
		await ToSignal(GetTree(), "process_frame");
   		EventManager.TriggerEvent("CLOSE_ALL_MENUS");
		_playButton = GetNode<TextureButton>("MarginContainer/VBoxContainer/HBoxContainer/play_button");
		_settingsButton = GetNode<TextureButton>("MarginContainer/VBoxContainer/HBoxContainer/settings_button");
		_settingsPopup = GetNode<Panel>("SettingsMenu");
		
		_playButton.TextureNormal = GD.Load<Texture2D>("res://source/button_play.png");
		_playButton.Size = new Vector2(100, 100);
		_playButton.StretchMode = TextureButton.StretchModeEnum.KeepAspect;
		
		_settingsButton.TextureNormal = GD.Load<Texture2D>("res://source/button_settings.png");
		_settingsButton.CustomMinimumSize = new Vector2(100, 100);
		_settingsButton.StretchMode = TextureButton.StretchModeEnum.KeepAspect;
		
		_settingsButton.Pressed += () => EventManager.TriggerEvent("OPEN_MENU", "SettingsMenu");
		_playButton.Pressed += () => EventManager.TriggerEvent("START_GAME");
		
	}
}

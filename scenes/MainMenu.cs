using Godot;
using System;

public partial class MainMenu : Control{
	
	private TextureButton _playButton;
	private TextureButton _settingsButton;
	private PopupPanel _settingsPopup;
	
	public override void _Ready(){
		_playButton = GetNode<TextureButton>("MarginContainer/VBoxContainer/HBoxContainer/play_button");
		_settingsButton = GetNode<TextureButton>("MarginContainer/VBoxContainer/HBoxContainer/settings_button");
		 _settingsPopup = GetNode<PopupPanel>("PopupPanel");
		
		_playButton.TextureNormal = GD.Load<Texture2D>("res://source/button_play.png");
		_playButton.Size = new Vector2(100, 100);
		_playButton.StretchMode = TextureButton.StretchModeEnum.KeepAspect;
		
		// on future 
		//_playButton.TexturePressed = GD.Load<Texture2D>("res://source/button_play_pressed.png)";
		
		_settingsButton.TextureNormal = GD.Load<Texture2D>("res://source/button_settings.png");
		_settingsButton.CustomMinimumSize = new Vector2(100, 100);
		_settingsButton.StretchMode = TextureButton.StretchModeEnum.KeepAspect;
		// on future 
		//_playButton.TexturePressed = GD.Load<Texture2D>("res://source/button_settings_pressed.png");
		
		
		_settingsButton.Pressed += OnSettingsPressed;
		
	}
	
	private void OnSettingsPressed(){
		_settingsPopup.PopupCentered();
	}
}

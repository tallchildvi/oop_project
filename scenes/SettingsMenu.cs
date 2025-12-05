/// <file>
/// <summary>
/// SettingsMenu.cs - Implements the functional logic for the game's settings interface.
/// </summary>
/// <remarks>
/// This script handles the initialization and styling of settings UI controls,
/// including volume sliders and language selectors. It manages user input via
/// button presses and control value changes, logging the output and communicating
/// changes through the <see cref="EventManager"/>.
/// </remarks>
/// </file>
using Godot;
using System;

/// <summary>
/// Represents the in-game settings menu. It inherits from <c>BaseMenu</c> 
/// to utilize standardized menu opening and closing mechanisms.
/// </summary>
public partial class SettingsMenu : BaseMenu
{
	/// <summary>
    /// The horizontal slider control used to adjust the music volume.
    /// </summary>
	private HSlider _musicSlider;

	/// <summary>
    /// The horizontal slider control (currently unused in logic) typically meant for adjusting sound effects volume.
    /// </summary>
	private HSlider _sfxSlider;

	/// <summary>
    /// The dropdown control used for selecting the game language.
    /// </summary>
	private OptionButton _languageSelector;

	/// <summary>
    /// The button used to close the settings menu and return to the previous screen.
    /// </summary>
	private Button _backBtn;

	/// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// Initializes the menu name, retrieves UI nodes, applies custom styles,
    /// sets initial values, and connects control signals to handler methods.
    /// </summary>
   public override void _Ready()
	{
		MenuName = "SettingsMenu";
		base._Ready();
		GD.Print("SettingsMenu Ready");

		// --- Node Retrieval ---
		_musicSlider = GetNodeOrNull<HSlider>("VBoxContainer/music_box/HSlider");
		// _sfxSlider is declared but not retrieved here
		_languageSelector = GetNodeOrNull<OptionButton>("VBoxContainer/language_box/OptionButton");
		_backBtn = GetNodeOrNull<Button>("VBoxContainer/Button");
		
		// --- Error Checking ---
		if (_musicSlider == null) GD.PrintErr("MusicSlider not found!");
		if (_languageSelector == null) GD.PrintErr("LanguageSelector not found!");
		if (_backBtn == null) GD.PrintErr("Back button not found!");

		// --- Slider Styling ---
        // Initializes custom StyleBoxFlat instances for visual overrides.
		var lineStyle = new StyleBoxFlat { BgColor = new Color(0.3f, 0.3f, 0.3f)};
		var fillStyle = new StyleBoxFlat { BgColor = new Color(0.2f, 0.8f, 0.2f)};
		var grabberStyle = new StyleBoxFlat { BgColor = new Color(1, 1, 1)};
		
		lineStyle.ContentMarginTop = 6;
		lineStyle.ContentMarginBottom = 6;
		
		// Checks again to prevent null reference exceptions during styling
		if (_musicSlider == null)
		{
			GD.PrintErr("_volumeSlider is null! Check export or GetNode path.");
			return;
		}

		_musicSlider.AddThemeStyleboxOverride("slider", lineStyle);
		_musicSlider.AddThemeStyleboxOverride("slider_fill", fillStyle);
		_musicSlider.AddThemeStyleboxOverride("grabber", grabberStyle);
		_musicSlider.AddThemeConstantOverride("grabber_offset", 0);

		// --- Initial Values ---
		_musicSlider.Value = 100;
		_languageSelector.Selected = 0;

		// --- Signal Connections ---
        /// <summary>
        /// Connects the <see cref="Range.ValueChanged"/> signal of the music slider to the handler method.
        /// </summary>
		_musicSlider.ValueChanged += OnMusicVolumeChanged;

		/// <summary>
        /// Connects the <see cref="OptionButton.ItemSelected"/> signal of the language selector to the handler method.
        /// </summary>
		_languageSelector.ItemSelected += OnLanguageSelected;

		/// <summary>
        /// Connects the <see cref="Button.Pressed"/> signal of the back button to trigger the "CLOSE_MENU" event.
        /// </summary>
		_backBtn.Pressed += () => EventManager.TriggerEvent("CLOSE_MENU", "SettingsMenu");
	}

	/// <summary>
    /// Handler method for when the music volume slider's value changes.
    /// </summary>
    /// <param name="value">The new volume value (as a <see cref="double"/>) from the slider.</param>
	private void OnMusicVolumeChanged(double value)
	{
		GD.Print("Music volume: ", value);
		// Note: Actual volume change implementation (e.g., using AudioServer.SetBusVolumeDb) is omitted here.
	}

	/// <summary>
    /// Handler method for when a new language is selected from the dropdown.
    /// </summary>
    /// <param name="index">The zero-based index of the newly selected item.</param>
	private void OnLanguageSelected(long index)
	{
		var lang = _languageSelector.GetItemText((int)index);
		GD.Print("Language selected: ", lang);
	}
}

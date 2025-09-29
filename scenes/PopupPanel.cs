using Godot;
using System;

public partial class PopupPanel : Godot.PopupPanel{
	private HSlider _musicSlider;
	private HSlider _sfxSlider;
	private OptionButton _languageSelector;
	private Button _backBtn;

   public override void _Ready()
	{
		// Підтягування вузлів
		_musicSlider = GetNode<HSlider>("VBoxContainer/music_box/HSlider");
		_languageSelector = GetNode<OptionButton>("VBoxContainer/language_box/OptionButton");
		_backBtn = GetNode<Button>("VBoxContainer/Button");

		if (_musicSlider == null) GD.PrintErr("MusicSlider not found!");
		if (_languageSelector == null) GD.PrintErr("LanguageSelector not found!");
		if (_backBtn == null) GD.PrintErr("Back button not found!");

		// Стиль слайдера
		var lineStyle = new StyleBoxFlat { BgColor = new Color(0.3f, 0.3f, 0.3f)};
		var fillStyle = new StyleBoxFlat { BgColor = new Color(0.2f, 0.8f, 0.2f)};
		var grabberStyle = new StyleBoxFlat { BgColor = new Color(1, 1, 1)};
		
		lineStyle.ContentMarginTop = 6;
		lineStyle.ContentMarginBottom = 6;

		_musicSlider.AddThemeStyleboxOverride("slider", lineStyle);
		_musicSlider.AddThemeStyleboxOverride("slider_fill", fillStyle);
		_musicSlider.AddThemeStyleboxOverride("grabber", grabberStyle);
		_musicSlider.AddThemeConstantOverride("grabber_offset", 0);

		// Початкове значення
		_musicSlider.Value = 100;
		_languageSelector.Selected = 0;

		// Сигнали
		_musicSlider.ValueChanged += OnMusicVolumeChanged;
		_languageSelector.ItemSelected += OnLanguageSelected;
		_backBtn.Pressed += () =>
		{
			GD.Print("Back pressed → hiding popup");
			Hide();
		};
	}

	private void OnMusicVolumeChanged(double value)
	{
		GD.Print("Music volume: ", value);
		// Тут можна підключити AudioServer.SetBusVolumeDb(...) для реальної зміни гучності
	}

	private void OnLanguageSelected(long index)
	{
		var lang = _languageSelector.GetItemText((int)index);
		GD.Print("Language selected: ", lang);
	}
}

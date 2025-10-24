using Godot;
using System;
using System.Collections.Generic;

public partial class MenuManager : Node
{
	public static MenuManager Instance { get; private set;}
	// private BaseMenu activeMenu;
	private Dictionary<string, BaseMenu> menus = new Dictionary<string, BaseMenu>();
	public override void _Ready()
	{
		Instance = this;
		EventManager.Subscribe("OPEN_MENU", OnOpenMenu);
		EventManager.Subscribe("CLOSE_MENU", OnCloseMenu);
		EventManager.Subscribe("TOGGLE_MENU", OnToggleMenu);
		EventManager.Subscribe("CLOSE_ALL_MENUS", OnCloseAll);
	}
	public void RegisterMenu(string name, BaseMenu menu)
	{
		menus[name] = menu;
	}
	// дописати else 
	private void OnOpenMenu(object param)
	{
		if (param is string name && menus.TryGetValue(name, out var menu))
		{
			menu.Open();
		}
	}
	private void OnCloseMenu(object param)
	{
		if (param is string name && menus.TryGetValue(name, out var menu))
		{
			menu.Close();
		}
	}
	private void OnToggleMenu(object param)
	{
		if (param is string name && menus.TryGetValue(name, out var menu))
		{
			menu.Toggle();
		}
	}
	private void OnCloseAll(object _)
	{
		foreach (BaseMenu menu in menus.Values)
		{
			menu.Close();
		}
	 	GD.Print("all menu have closed");
	}

	public override void _ExitTree()
	{
		EventManager.Unsubscribe("OPEN_MENU", OnOpenMenu);
		EventManager.Unsubscribe("CLOSE_MENU", OnCloseMenu);
		EventManager.Unsubscribe("TOGGLE_MENU", OnToggleMenu);
		EventManager.Unsubscribe("CLOSE_ALL_MENUS", OnCloseAll);
	}
}

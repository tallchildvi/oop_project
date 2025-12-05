/// <file>
/// <summary>
/// MenuManager.cs - Manages the state and control of all UI menus in the game.
/// </summary>
/// <remarks>
/// This file provides the implementation of the singleton <see cref="MenuManager"/> class.
/// It acts as a centralized access point for registering and controlling menus.
/// It relies on the <see cref="EventManager"/> to listen for global UI control requests (OPEN_MENU, CLOSE_MENU, etc.).
/// </remarks>
/// </file>
using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// A singleton manager responsible for registering, tracking, and controlling all game menus.
/// It uses a publish/subscribe pattern via the <see cref="EventManager"/> to react to menu requests.
/// </summary>
public partial class MenuManager : Node
{
	/// <summary>
    /// Gets the singleton instance of the <see cref="MenuManager"/>.
    /// </summary>
    /// <value>The static instance of the manager.</value>
	public static MenuManager Instance { get; private set;}
	// private BaseMenu activeMenu;
	/// <summary>
    /// A dictionary storing all registered menus, keyed by their unique string names.
    /// The value is the instance of the menu, expected to inherit from <c>BaseMenu</c> (which is not defined here but implied by usage).
    /// </summary>
	private Dictionary<string, BaseMenu> menus = new Dictionary<string, BaseMenu>();
	
	/// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// Initializes the singleton <see cref="Instance"/> and subscribes to necessary events
    /// from the <see cref="EventManager"/>.
    /// </summary>
	public override void _Ready()
	{
		Instance = this;
		EventManager.Subscribe("OPEN_MENU", OnOpenMenu);
		EventManager.Subscribe("CLOSE_MENU", OnCloseMenu);
		EventManager.Subscribe("TOGGLE_MENU", OnToggleMenu);
		EventManager.Subscribe("CLOSE_ALL_MENUS", OnCloseAll);
	}

	/// <summary>
    /// Registers a menu instance with a specific name, making it available for control.
    /// </summary>
    /// <param name="name">The unique string name to identify the menu.</param>
    /// <param name="menu">The menu instance, which must inherit from <c>BaseMenu</c>.</param>
	public void RegisterMenu(string name, BaseMenu menu)
	{
		menus[name] = menu;
	}
	// дописати else 
	
	/// <summary>
    /// Handles the "OPEN_MENU" event. Attempts to open the menu identified by the provided parameter name.
    /// </summary>
    /// <param name="param">The event parameter, expected to be a <see cref="string"/> containing the menu name.</param>
	private void OnOpenMenu(object param)
	{
		if (param is string name && menus.TryGetValue(name, out var menu))
		{
			menu.Open();
		}
	}

	/// <summary>
    /// Handles the "CLOSE_MENU" event. Attempts to close the menu identified by the provided parameter name.
    /// </summary>
    /// <param name="param">The event parameter, expected to be a <see cref="string"/> containing the menu name.</param>
	private void OnCloseMenu(object param)
	{
		if (param is string name && menus.TryGetValue(name, out var menu))
		{
			menu.Close();
		}
	}

	/// <summary>
    /// Handles the "TOGGLE_MENU" event. Attempts to toggle the open/closed state of the menu identified by the provided parameter name.
    /// </summary>
    /// <param name="param">The event parameter, expected to be a <see cref="string"/> containing the menu name.</param>
	private void OnToggleMenu(object param)
	{
		if (param is string name && menus.TryGetValue(name, out var menu))
		{
			menu.Toggle();
		}
	}

	/// <summary>
    /// Handles the "CLOSE_ALL_MENUS" event. Iterates through all registered menus and closes them.
    /// </summary>
    /// <param name="_">The event parameter (ignored).</param>
	private void OnCloseAll(object _)
	{
		foreach (BaseMenu menu in menus.Values)
		{
			menu.Close();
		}
	 	GD.Print("all menu have closed");
	}

	/// <summary>
    /// Called when the node is about to exit the scene tree.
    /// Unsubscribes from all <see cref="EventManager"/> events to prevent errors.
    /// </summary>
	public override void _ExitTree()
	{
		EventManager.Unsubscribe("OPEN_MENU", OnOpenMenu);
		EventManager.Unsubscribe("CLOSE_MENU", OnCloseMenu);
		EventManager.Unsubscribe("TOGGLE_MENU", OnToggleMenu);
		EventManager.Unsubscribe("CLOSE_ALL_MENUS", OnCloseAll);
	}
}

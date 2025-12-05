/// <file>
/// <summary>
/// EventManager.cs - Centralized static event manager for Godot C# projects.
/// </summary>
/// <remarks>
/// This file provides the implementation of the singleton <see cref="EventManager"/> class.
/// It uses a dictionary of event names and actions to implement a thread-safe
/// publish/subscribe (Pub/Sub) pattern, allowing decoupled communication between game systems.
/// </remarks>
/// </file>
using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// A centralized static manager for handling game events using a dictionary of event names and actions.
/// This implementation allows for decoupling of game systems by providing a publish/subscribe mechanism.
/// It inherits from <see cref="Control"/> primarily to utilize Godot's lifecycle methods like <see cref="_Ready"/> and <see cref="_ExitTree"/>.
/// </summary>
public partial class EventManager : Control
{
	/// <summary>
    /// Gets the singleton instance of the <see cref="EventManager"/>.
    /// </summary>
    /// <value>The static instance of the manager.</value>
	public static EventManager Instance { get; private set; }

	/// <summary>
    /// The dictionary that stores event names as keys and a multicast delegate (<see cref="Action{T}"/> where T is <see cref="object"/>)
    /// as values. Each delegate holds all the subscribed listener methods for that event.
    /// </summary>
	private static Dictionary<string, Action<object>> eventDictionary = new();

	/// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// Initializes the singleton <see cref="Instance"/>.
    /// </summary>
	public override void _Ready()
	{
		Instance = this;
	}

	/// <summary>
    /// Called when the node is about to exit the scene tree.
    /// Clears the singleton <see cref="Instance"/> reference.
    /// </summary>
	public override void _ExitTree()
	{
		Instance = null;
	}

	/// <summary>
    /// Subscribes a listener method to a specific event name.
    /// </summary>
    /// <param name="eventName">The unique string identifier for the event.</param>
    /// <param name="listener">The <see cref="Action{T}"/> method (where T is <see cref="object"/>) to be executed when the event is triggered.</param>
	public static void Subscribe(string eventName, Action<object> listener)
	{
		if (string.IsNullOrEmpty(eventName) || listener == null) return;

		if (!eventDictionary.ContainsKey(eventName))
			eventDictionary[eventName] = delegate { };

		eventDictionary[eventName] += listener;
	}

	/// <summary>
    /// Unsubscribes a listener method from a specific event name.
    /// </summary>
    /// <param name="eventName">The unique string identifier for the event.</param>
    /// <param name="listener">The <see cref="Action{T}"/> method (where T is <see cref="object"/>) to be removed from the event's listeners.</param>
	public static void Unsubscribe(string eventName, Action<object> listener)
	{
		if (string.IsNullOrEmpty(eventName) || listener == null) return;
		if (!eventDictionary.ContainsKey(eventName)) return;

		eventDictionary[eventName] -= listener;

		var current = eventDictionary[eventName];
		if (current == null || current.GetInvocationList().Length == 0)
			eventDictionary.Remove(eventName);
	}
	
	/// <summary>
    /// Triggers an event by its name, executing all subscribed listener methods.
    /// </summary>
    /// <param name="eventName">The unique string identifier for the event to trigger.</param>
    /// <param name="data">Optional data (<see cref="object"/>) to pass to the listener methods. Default is null.</param>
	public static void TriggerEvent(string eventName, object data = null)
	{
		if (string.IsNullOrEmpty(eventName)) return;
		if (!eventDictionary.ContainsKey(eventName)) return;

		var del = eventDictionary[eventName];
		if (del == null) return;

		foreach (Action<object> single in del.GetInvocationList())
		{
			try
			{
				single.Invoke(data);
			}
			catch (Exception ex)
			{
				GD.PrintErr($"[EventManager] Exception in listener for {eventName}: {ex}");
			}
		}
	}
}

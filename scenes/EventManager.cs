using Godot;
using System;
using System.Collections.Generic;

public partial class EventManager : Control
{
	public static EventManager Instance { get; private set; }
	private static Dictionary<string, Action<object>> eventDictionary = new();

	public override void _Ready()
	{
		Instance = this;
	}

	public override void _ExitTree()
	{
		// Не очищаємо глобально весь словник тут — інші системи можуть чекати подій.
		Instance = null;
	}

	public static void Subscribe(string eventName, Action<object> listener)
	{
		if (string.IsNullOrEmpty(eventName) || listener == null) return;

		if (!eventDictionary.ContainsKey(eventName))
			eventDictionary[eventName] = delegate { };

		eventDictionary[eventName] += listener;
	}

	public static void Unsubscribe(string eventName, Action<object> listener)
	{
		if (string.IsNullOrEmpty(eventName) || listener == null) return;
		if (!eventDictionary.ContainsKey(eventName)) return;

		eventDictionary[eventName] -= listener;

		var current = eventDictionary[eventName];
		if (current == null || current.GetInvocationList().Length == 0)
			eventDictionary.Remove(eventName);
	}

	public static void TriggerEvent(string eventName, object data = null)
	{
		if (string.IsNullOrEmpty(eventName)) return;
		if (!eventDictionary.ContainsKey(eventName)) return;

		var del = eventDictionary[eventName];
		if (del == null) return;

		// Безпечний виклик — щоб помилка одного підписника не перервала всіх
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

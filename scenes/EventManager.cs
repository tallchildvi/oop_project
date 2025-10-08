using Godot;
using System;
using System.Collections.Generic;


public partial class EventManager : Control
{
	//щоб мати доступ до дерева подій і наслідувати node
	public static EventManager Instance { get; private set; }
	private static Dictionary<string, Action<object>> eventDictionary = new();

	public override void _Ready()
	{
		Instance = this;
	}

	public override void _ExitTree()
	{
		eventDictionary.Clear();
	}

	public static void Subscribe(string eventName, Action<object> listener)
	{
		if (!eventDictionary.ContainsKey(eventName))
		{
			eventDictionary[eventName] = delegate { };
		}
		eventDictionary[eventName] += listener;
	}
	public static void Unsubscribe(string eventName, Action<object> listener)
	{
		if (eventDictionary.ContainsKey(eventName))
		{
			eventDictionary[eventName] -= listener;
		}
	}

	public static void TriggerEvent(string eventName, object data = null)
	{
		if (eventDictionary.ContainsKey(eventName))
		{
			GD.Print("trigger");
			eventDictionary[eventName]?.Invoke(data);
		}
	}
}

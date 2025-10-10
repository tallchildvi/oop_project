using Godot;
using System;
using System.Threading.Tasks;

public partial class Trader : Area2D
{
	private Vector2 screenCenter;
	private CharacterManager manager;
	private float entrySpeed = 200f;
	private bool placed = false;

	public override void _Ready()
	{
		Rect2 viewportRect = GetViewportRect();
		screenCenter = viewportRect.Size / 2f;

		Connect("area_entered", new Callable(this, nameof(OnAreaEntered)));
		manager = GetParent().GetNode<CharacterManager>("CharacterManager");

		_ = Enter();
	}

	private async Task Enter()
	{
		while (Mathf.Abs(Position.X - screenCenter.X) > 1f)
		{
			float direction = Position.X > screenCenter.X ? -1 : 1;
			Position += new Vector2(direction, 0) * entrySpeed * (float)GetProcessDeltaTime();
			await ToSignal(GetTree(), "physics_frame");
		}

		placed = true;
		GD.Print("Trader reached center!");
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area == null) return;

		Node current = area;
		while (current != null && current is not MainCharacter)
			current = current.GetParent();

		if (current is MainCharacter)
			manager.allow_shop = true;
	}

	public async Task Exit(float exitSpeed = 300f)
	{
		while (GlobalPosition.X > -100f)
		{
			GlobalPosition += Vector2.Left * exitSpeed * (float)GetProcessDeltaTime();
			await ToSignal(GetTree(), "physics_frame");
		}

		QueueFree();
	}

}

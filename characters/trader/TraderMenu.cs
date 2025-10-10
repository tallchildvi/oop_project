using Godot;
using System;
using System.Threading.Tasks;

public partial class TraderMenu : Node2D
{
	[Export] public CharacterManager manager;
	private Button close_btn;
	private Button spin_btn;
	private Sprite2D spinerSprite;
	private Trader trader;
	
	
	public override void _Ready()
	{
		spinerSprite = GetNode<Sprite2D>("spiner");
		spin_btn = GetNode<Button>("spin_btn");
		spin_btn.Pressed += async () => await Spin();
		close_btn = GetNode<Button>("close_btn");
		trader = FindTrader();
		close_btn.Pressed += () =>
		{
			if (trader != null)
			{
				trader.Exit();
			}
			if (GetParent() is CharacterManager manager)
			{
				manager.shop_exists = false;
			}
			QueueFree();
		};

	}

	private Trader FindTrader()
	{
		foreach (Node node in GetTree().GetRoot().GetChildren())
		{
			Trader t = SearchTrader(node);
			if (t != null)
				return t;
		}
		return null;
	}

	private Trader SearchTrader(Node node)
	{
		if (node is Trader t)
			return t;
		foreach (Node child in node.GetChildren())
		{
			Trader found = SearchTrader(child);
			if (found != null)
				return found;
		}
		return null;
	}

	private async Task Spin()
	{
		RandomNumberGenerator rng = new RandomNumberGenerator();
		rng.Randomize();

		float startAngle = spinerSprite.RotationDegrees;
		float totalRotation = rng.RandfRange(720f, 1440f);
		float targetAngle = startAngle + totalRotation;

		float duration = 2f;
		float elapsed = 0f;

		while (elapsed < duration)
		{
			float t = elapsed / duration;
			float easedT = 1 - Mathf.Pow(1 - t, 3);
			spinerSprite.RotationDegrees = Mathf.Lerp(startAngle, targetAngle, easedT);

			await ToSignal(GetTree(), "process_frame");
			elapsed += (float)GetProcessDeltaTime();
		}

		spinerSprite.RotationDegrees = targetAngle % 360f;

		int item = Mathf.FloorToInt(spinerSprite.RotationDegrees / 90f) % 4;

		manager.luck(item);
	}
}

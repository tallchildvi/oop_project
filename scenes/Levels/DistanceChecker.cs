using Godot;

public class DistanceChecker
{
	private Node2D owner;
	private float _threshold;

	public bool IsActive { get; private set; } = false;

	public DistanceChecker(Node2D owner, float threshold = 400f)
	{
		this.owner = owner;
		this._threshold = threshold;
	}

	public void SetThreshold(float t) => _threshold = t;

	public void CheckDistance(Vector2 playerPosition)
	{
		if (owner == null) return;

		float distance = owner.GlobalPosition.DistanceTo(playerPosition);
		bool nowActive = distance < _threshold;

		if (nowActive != IsActive)
			IsActive = nowActive;
	}
}

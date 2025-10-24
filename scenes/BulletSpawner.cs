using Godot;
using System;

public class BulletSpawner
{
	private Node2D owner;
	private PackedScene bulletScene;
	private string bulletGroup;

	public BulletSpawner(Node2D owner, PackedScene bulletScene, string bulletGroup = "")
	{
		this.owner = owner;
		this.bulletScene = bulletScene;
		this.bulletGroup = bulletGroup ?? "";
	}

	public void SpawnBullet(Node2D player, bool facingRight)
	{
		if (bulletScene == null || owner == null) return;

		var node = bulletScene.Instantiate();
		if (node == null) return;

		if (node is BulletInheritance bullet)
		{
			bullet.GlobalPosition = owner.GlobalPosition;
			Vector2 direction = Vector2.Right;
			if (player != null)
				direction = (player.GlobalPosition - owner.GlobalPosition).Normalized();
			else
				direction = facingRight ? Vector2.Right : Vector2.Left;

			bullet.Init(direction, facingRight);

			if (!string.IsNullOrEmpty(bulletGroup))
				bullet.AddToGroup(bulletGroup);
				
			var root = owner.GetTree().CurrentScene;
			if (root != null)
				root.AddChild(bullet);
			else
				owner.GetParent()?.AddChild(bullet);
			return;
		}

		var rootFallback = owner.GetTree().CurrentScene;
		if (rootFallback != null)
			rootFallback.AddChild(node);
		else
			owner.GetParent()?.AddChild(node);

		GD.PrintErr("[BulletSpawner] Spawned bullet instance does not inherit BulletInheritance. Ensure prefab matches expected type.");
	}
}

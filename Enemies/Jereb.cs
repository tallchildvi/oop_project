//using Godot;
//
//public partial class Jereb : Area2D
//{
	//private bool facingRight = true;
	//private Sprite2D characterSprite;
	//private float bulletSpawnInterval = 3.0f;
	//private float timeSinceLastBullet = 0f;
	//private PackedScene bulletScene;
	//private Node2D player;
	//private DistanceChecker distanceChecker;
	//private BulletSpawner bulletSpawner;
//
	//public override void _Ready()
	//{
		//characterSprite = GetNode<Sprite2D>("Sprite2D");
		//facingRight = true;
		//bulletScene = ResourceLoader.Load<PackedScene>("res://enemies/jereb/EnemyBullet.tscn");
		//AddToGroup("enemy");
		//distanceChecker = new DistanceChecker(this);
		//bulletSpawner = new BulletSpawner(this, bulletScene);
	//}
//
	//public override void _Process(double delta)
	//{
		//player = GetNode<Node2D>("/root/Node2D/CharacterManager/main_character/Character2D");
		//if (player == null) return;
//
		//distanceChecker.CheckDistance(player.GlobalPosition, characterSprite, ref facingRight);
//
		//if (distanceChecker.IsActive)
		//{
			//timeSinceLastBullet += (float)delta;
			//if (timeSinceLastBullet >= bulletSpawnInterval)
			//{
				//bulletSpawner.SpawnBullet(player, facingRight);
				//timeSinceLastBullet = 0f;
			//}
		//}
	//}
//}
//

using Godot;
using System;
public class DistanceChecker
{
	private Node2D owner;
	private const float DistanceThreshold = 400f;
	private bool isActive = false;
	private ActivationHandler activationHandler = new ActivationHandler();

	public bool IsActive => isActive;

	public DistanceChecker(Node2D owner)
	{
		this.owner = owner;
	}

	public void CheckDistance(Vector2 playerPosition, AnimatedSprite2D sprite, ref bool facingRight)
	{
		float distance = owner.GlobalPosition.DistanceTo(playerPosition);

		if (distance < DistanceThreshold && !isActive)
		{
			activationHandler.Activate(sprite, ref facingRight);
			isActive = true;
		}
		else if (distance >= DistanceThreshold && isActive)
		{
			activationHandler.Deactivate(sprite, ref facingRight);
			isActive = false;
		}
	}
}

public class ActivationHandler
{
	public void Activate(AnimatedSprite2D sprite, ref bool facingRight)
	{
		facingRight = false;
		sprite.FlipH = true;
	}

	public void Deactivate(AnimatedSprite2D sprite, ref bool facingRight)
	{
		facingRight = true;
		sprite.FlipH = false;
	}
}

public class BulletSpawner
{
	private Node2D owner;
	private PackedScene bulletScene;

	public BulletSpawner(Node2D owner, PackedScene bulletScene)
	{
		this.owner = owner;
		this.bulletScene = bulletScene;
	}

	public void SpawnBullet(Node2D player, bool facingRight)
	{
		if (bulletScene == null) return;

		var bulletNode = bulletScene.Instantiate();
		if (bulletNode is EnemyBullet bullet)
		{
			bullet.Position = owner.GlobalPosition;
			Vector2 direction = (player.GlobalPosition - owner.GlobalPosition).Normalized();
			bullet.Init(direction, facingRight);
			owner.GetParent().AddChild(bullet);
		}
	}
}



public partial class Jereb : BaseEnemy
{
	private AnimatedSprite2D characterSprite;
	[Export] private float bulletSpawnInterval = 3.0f;
	private float timeSinceLastBullet = 0f;
	[Export] private PackedScene bulletScene;

	private DistanceChecker distanceChecker;
	private BulletSpawner bulletSpawner;

	public override void _Ready()
	{
		characterSprite = GetNode<AnimatedSprite2D>("EnemySprite");
		AddToGroup("enemy");

		distanceChecker = new DistanceChecker(this);
		bulletSpawner = new BulletSpawner(this, bulletScene);
	}

	public override void _Process(double delta)
	{
		if (Player == null) return;

		distanceChecker.CheckDistance(Player.GlobalPosition, characterSprite, ref FacingRight);

		if (distanceChecker.IsActive)
		{
			timeSinceLastBullet += (float)delta;
			if (timeSinceLastBullet >= bulletSpawnInterval)
			{
				bulletSpawner.SpawnBullet(Player, FacingRight);
				timeSinceLastBullet = 0f;
			}
		}
	}
}

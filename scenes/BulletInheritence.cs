using Godot;
using System;

public partial class BulletInheritance : Area2D
{
	protected BulletMovement movementLogic;
	protected BulletCollisionHandler collisionHandler;
	protected bool facingRight;
	protected float speed = 400f;
	protected string selfGroup = "";
	protected string oppositeGroup = "";

	public BulletInheritance()
	{
		movementLogic = new BulletMovement(this, speed);
		collisionHandler = new BulletCollisionHandler(this);
	}

	public virtual void Init(Vector2 direction, bool facingRight)
	{
		this.facingRight = facingRight;
		movementLogic.SetDirection(direction, facingRight);
	}

	public override void _Process(double delta)
	{
		movementLogic.Move(delta);
	}
	
	public override void _Ready()
	{
		base._Ready();
		movementLogic = new BulletMovement(this, speed);
		collisionHandler = new BulletCollisionHandler(this);
		this.AreaEntered += OnAreaEntered;
		if (!string.IsNullOrEmpty(selfGroup))
			AddToGroup(selfGroup);
	}

	//public override void _Ready()
	//{
		//
		//this.AreaEntered += OnAreaEntered;
		//if (!string.IsNullOrEmpty(selfGroup))
			//AddToGroup(selfGroup);
			//
	//}

	protected virtual void OnAreaEntered(Area2D area)
	{
		collisionHandler.HandleCollision(area, oppositeGroup);
	}
}

// Movement logic
public class BulletMovement
{
	private Node2D owner;
	private Vector2 direction = Vector2.Right;
	private float speed;

	public BulletMovement(Node2D owner, float speed)
	{
		this.owner = owner;
		this.speed = speed;
	}

	public void SetDirection(Vector2 dir, bool facingRight)
	{
		direction = dir;
		if (direction == Vector2.Zero)
			direction = facingRight ? Vector2.Right : Vector2.Left;
		direction = direction.Normalized();
	}

	public void Move(double delta)
	{
		owner.Position += direction * speed * (float)delta;
		owner.Rotation = direction.Angle();
	}
}

public class BulletCollisionHandler
{
	private Area2D owner;

	public BulletCollisionHandler(Area2D owner)
	{
		this.owner = owner;
	}

	public void HandleCollision(Area2D area, string oppositeGroup)
	{
		if (area == null || owner == null) return;

		if (owner.IsInGroup("enemy_bullet") && area.IsInGroup("player"))
		{
			if (area is BaseCharacter player)
				player.TakeDamage(1);
			owner.QueueFree();
			return;
		}
		
		if (owner.IsInGroup("player_bullet") && area.IsInGroup("enemy"))
		{
			if (area is BaseEnemy enemy)
				enemy.TakeDamage(1);
			owner.QueueFree();
			return;
		}

		if (!string.IsNullOrEmpty(oppositeGroup) && area.IsInGroup(oppositeGroup))
		{
			owner.QueueFree();
		}
	}
}

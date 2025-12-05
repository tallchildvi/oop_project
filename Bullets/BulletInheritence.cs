/// <file>
/// <summary>
/// BulletInheritance.cs - Base projectile implementation using composition
/// </summary>
/// </file>

using Godot;
using System;

/// <summary>
/// Base class for game projectiles (bullets).
/// </summary>
/// <remarks>
/// Acts as a controller that inherits from Area2D.
/// Delegates specific logic to <see cref="BulletMovement"/> for translation 
/// and <see cref="BulletCollisionHandler"/> for impact resolution.
/// </remarks>
public partial class BulletInheritance : Area2D
{
    /// <summary>Component responsible for calculating position and rotation.</summary>
    protected BulletMovement movementLogic;
    
    /// <summary>Component responsible for resolving impacts.</summary>
    protected BulletCollisionHandler collisionHandler;
    
    /// <summary>State tracking if the bullet was fired while facing right.</summary>
    protected bool facingRight;
    
    /// <summary>Movement speed in pixels per second.</summary>
    protected float speed = 1000f;
    
    /// <summary>Group name for this bullet (e.g., "player_bullet").</summary>
    protected string selfGroup = "";
    
    /// <summary>Target group to check collisions against (e.g., "enemy").</summary>
    protected string oppositeGroup = "";

    /// <summary>
    /// Default constructor.
    /// </summary>
    public BulletInheritance()
    {
        //movementLogic = new BulletMovement(this, speed);
        //collisionHandler = new BulletCollisionHandler(this);
    }

    /// <summary>
    /// Initializes the bullet with direction and creates logic components.
    /// </summary>
    /// <param name="direction">The initial flight vector.</param>
    /// <param name="facingRight">True if the shooter acts facing right.</param>
    public virtual void Init(Vector2 direction, bool facingRight)
    {
        movementLogic = new BulletMovement(this, speed);
        collisionHandler = new BulletCollisionHandler(this);
        this.facingRight = facingRight;
        movementLogic.SetDirection(direction, facingRight);
    }

    /// <summary>
    /// Called every frame. Updates bullet position.
    /// </summary>
    /// <param name="delta">Time elapsed since the last frame.</param>
    public override void _Process(double delta)
    {
        movementLogic.Move(delta);
    }
    
    /// <summary>
    /// Called when the node enters the scene tree. Configures collision signals.
    /// </summary>
    public override void _Ready()
    {
        base._Ready();
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

    /// <summary>
    /// Signal callback for when this area overlaps another area.
    /// </summary>
    /// <param name="area">The area that was hit.</param>
    protected virtual void OnAreaEntered(Area2D area)
    {
        collisionHandler.HandleCollision(area, oppositeGroup);
    }
}

/// <summary>
/// Encapsulates movement logic for projectiles.
/// </summary>
public class BulletMovement
{
    /// <summary>The visual/physical node to move.</summary>
    protected Node2D owner;
    
    /// <summary>Normalized movement vector.</summary>
    public Vector2 direction = Vector2.Right;
    
    /// <summary>Speed scalar.</summary>
    protected float speed;

    /// <summary>
    /// Creates a movement component.
    /// </summary>
    /// <param name="owner">The node to manipulate.</param>
    /// <param name="speed">Movement speed.</param>
    public BulletMovement(Node2D owner, float speed)
    {
        this.owner = owner;
        this.speed = speed;
    }

    /// <summary>
    /// Calculates and sets the normalized direction vector.
    /// </summary>
    /// <param name="dir">Raw input direction.</param>
    /// <param name="facingRight">Fallback direction if dir is Zero.</param>
    /// <remarks>
    /// Also logs the direction to console for debugging.
    /// </remarks>
    public void SetDirection(Vector2 dir, bool facingRight)
    {
        GD.Print($"[BulletInheritence] set direction for bullet {dir}");
        direction = dir;
        if (direction == Vector2.Zero)
            direction = facingRight ? Vector2.Right : Vector2.Left;
        direction = direction.Normalized();
    }

    /// <summary>
    /// Applies movement and rotation to the owner node.
    /// </summary>
    /// <param name="delta">Time slice.</param>
    public void Move(double delta)
    {
        owner.Position += direction * speed * (float)delta;
        owner.Rotation = direction.Angle();
    }
}

/// <summary>
/// Encapsulates collision and damage logic for projectiles.
/// </summary>
public class BulletCollisionHandler
{
    /// <summary>The bullet area causing the collision.</summary>
    private Area2D owner;

    /// <summary>
    /// Creates a collision handler.
    /// </summary>
    /// <param name="owner">The Area2D to manage.</param>
    public BulletCollisionHandler(Area2D owner)
    {
        this.owner = owner;
    }

    /// <summary>
    /// Processes the collision event, deals damage, and destroys the bullet.
    /// </summary>
    /// <param name="area">The target object hit.</param>
    /// <param name="oppositeGroup">General group name for enemies.</param>
    /// <remarks>
    /// Checks for "enemy_bullet" vs "player" and "player_bullet" vs "enemy" interactions.
    /// Casts targets to BaseCharacter or BaseEnemy to apply damage.
    /// </remarks>
    public void HandleCollision(Area2D area, string oppositeGroup)
    {
        if (area == null || owner == null) return;

        // Enemy bullet hitting Player
        if (owner.IsInGroup("enemy_bullet") && area.IsInGroup("player"))
        {
            if (area is BaseCharacter player)
                player.TakeDamage(1);
            owner.QueueFree();
            return;
        }
        
        // Player bullet hitting Enemy
        if (owner.IsInGroup("player_bullet") && area.IsInGroup("enemy"))
        {
            if (area is BaseEnemy enemy)
                enemy.TakeDamage(1);
            owner.QueueFree();
            return;
        }

        // Generic collision with opposite group
        if (!string.IsNullOrEmpty(oppositeGroup) && area.IsInGroup(oppositeGroup))
        {
            owner.QueueFree();
        }
    }
}
/// <file>
/// <summary>
/// BaseEnemy.cs - Provides the base class for all enemy entities in the game.
/// </summary>
/// <remarks>
/// This class handles fundamental enemy properties (health, speed, movement),
/// core mechanics (taking damage, dying, pausing/resuming), and basic movement
/// logic (simple pursuit of the player). It inherits from <see cref="Area2D"/> for collision detection.
/// </remarks>
/// </file>
using Godot;
using System;

/// <summary>
/// The abstract base class for all hostile entities (enemies) in the game.
/// It provides core attributes, health management, movement, and lifecycle methods,
/// including integration with object pooling mechanisms via <see cref="Activate"/> and <see cref="ResetState"/>.
/// </summary>
public partial class BaseEnemy : Area2D
{
	/// <summary>
    /// The maximum health value of this enemy type.
    /// </summary>
    public int maxHealth = 2;

    /// <summary>
    /// The movement speed (units per second) of the enemy.
    /// </summary>
    public float speed = 50f;

    /// <summary>
    /// The current health of the enemy.
    /// </summary>
    protected int currentHealth;

    /// <summary>
    /// Flag indicating whether the enemy is currently dead.
    /// </summary>
    protected bool isDead;

    /// <summary>
    /// Flag indicating whether the enemy's logic and movement are suspended.
    /// </summary>
    protected bool isPaused = false;

    /// <summary>
    /// Flag indicating the visual direction the enemy is currently facing (true for right).
    /// </summary>
    protected bool facingRight = true;

    /// <summary>
    /// Gets or sets the target character (assumed to be the player).
    /// </summary>
    public BaseCharacter Player { get; protected set; }

    /// <summary>
    /// Event triggered when the enemy's health drops to zero or below.
    /// Subscribers (e.g., <c>EnemyManager</c>) are notified upon death.
    /// </summary>
    public event Action<BaseEnemy> Died;

    /// <summary>
    /// The normalized vector used for calculating movement direction, typically toward the <see cref="Player"/>.
    /// </summary>
    protected Vector2 movement = Vector2.Zero;

	/// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// Initializes health and attempts to find the player.
    /// </summary>
	public override void _Ready()
	{
		currentHealth = maxHealth;
		// Find the player node (assumes the player is in the "player" group).
		
		Player = GetTree().GetFirstNodeInGroup("player") as BaseCharacter;
		ProcessMode = ProcessModeEnum.Inherit;
	}

	/// <summary>
    /// Checks if the enemy is dead.
    /// </summary>
    /// <returns><c>true</c> if the enemy is dead; otherwise, <c>false</c>.</returns>
	public bool Dead(){
		return isDead;
	}

	/// <summary>
    /// Called every frame. Handles movement logic if the game is not paused and the player is present.
    /// Implements basic pursuit behavior towards the player's global position.
    /// </summary>
    /// <param name="delta">The elapsed time since the previous frame.</param>
	public override void _Process(double delta){
		if (isPaused) return; 
		if (Player == null) return;

		// Calculate direction vector towards the player
		movement = new Vector2(Player.GlobalPosition.X - Position.X, Player.GlobalPosition.Y - Position.Y);
		movement = movement.Normalized();// Normalize to get direction only

        // Apply movement
		Position += movement * speed * (float) delta;
	}

	/// <summary>
    /// Reduces the enemy's health by a specified amount and calls <see cref="Die"/> if health drops to zero or below.
    /// </summary>
    /// <param name="amount">The amount of damage to take.</param>
	public virtual void TakeDamage(int amount)
	{
		if (isDead) return;

		currentHealth -= amount;
		if (currentHealth <= 0)
			Die();
	}

	/// <summary>
    /// Handles the enemy's death sequence: sets the dead flag, invokes the <see cref="Died"/> event,
    /// and disables collision, visibility, and processing.
    /// </summary>
	protected virtual void Die()
	{
		if (isDead) return;
		isDead = true; 

		// Notify subscribers of death
		try
		{
			Died?.Invoke(this);
		}
		catch (Exception ex)
		{
			GD.PrintErr($"[BaseEnemy] Exception while invoking Died: {ex}");
		}

		// Disable collision, visibility, and processing (using SetDeferred for safety)
		var collision = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
		if (collision != null)
			collision.SetDeferred("disabled", true);

		SetDeferred("visible", false);
		SetDeferred("process_mode", (int)ProcessModeEnum.Disabled);
		Visible = false;
		ProcessMode = ProcessModeEnum.Disabled;
	}

	/// <summary>
    /// Activates the enemy for use, typically when retrieved from an object pool.
    /// Resets state, position, health, enables visibility, and enables collision.
    /// </summary>
    /// <param name="spawnPosition">The world position where the enemy should appear.</param>
	public virtual void Activate(Vector2 spawnPosition)
	{
		AddToGroup("enemy");
		Position = spawnPosition;
		Visible = true;
		isDead = false;
		currentHealth = maxHealth;
		facingRight = true;
		ProcessMode = ProcessModeEnum.Inherit;

		var collision = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
		if (collision != null) collision.SetDeferred("disabled", false);;
	}

	/// <summary>
    /// Resets the enemy's critical state variables to their initial values, making it ready for reuse.
    /// </summary>
	public virtual void ResetState()
	{
		currentHealth = maxHealth;
		isDead = false;
		isPaused = false;
		Visible = true;
		facingRight = true;
		ProcessMode = ProcessModeEnum.Inherit;

		var collision = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
		if (collision != null) collision.SetDeferred("disabled", false);
	}

	/// <summary>
    /// Suspends the enemy's internal logic and movement by setting <see cref="isPaused"/> to true
    /// and disabling the node's process mode.
    /// </summary>
	public virtual void Pause()
	{
		isPaused = true;
		ProcessMode = ProcessModeEnum.Disabled;
	}

	/// <summary>
    /// Resumes the enemy's internal logic and movement.
    /// </summary>
	public virtual void Resume()
	{
		isPaused = false;
		ProcessMode = ProcessModeEnum.Inherit;
	}
}

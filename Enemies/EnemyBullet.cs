/// <file>
/// <summary>
/// EnemyBullet.cs - Specific implementation for enemy projectiles
/// </summary>
/// </file>

using Godot;

/// <summary>
/// Concrete implementation of an enemy projectile.
/// </summary>
/// <remarks>
/// Inherits from <see cref="BulletInheritance"/>.
/// Automatically configures itself to identify as an "enemy_bullet" 
/// and interacts with "player_bullet" objects (e.g., for bullet cancellation).
/// </remarks>
public partial class EnemyBullet : BulletInheritance
{
    /// <summary>
    /// Configures default properties for the enemy bullet on scene entry.
    /// </summary>
    /// <remarks>
    /// Overrides base settings:
    /// <list type="bullet">
    /// <item>Self Group: "enemy_bullet"</item>
    /// <item>Opposite Group: "player_bullet"</item>
    /// </list>
    /// </remarks>
    public override void _Ready()
    {
        base._Ready();
        selfGroup = "enemy_bullet";
        oppositeGroup = "player_bullet";
        //movementLogic = new BulletMovement(this, 1000f);
    }

    /// <summary>
    /// Initializes the bullet logic and explicitly confirms group membership.
    /// </summary>
    /// <param name="direction">The calculated flight vector.</param>
    /// <param name="facingRight">The facing direction of the shooter.</param>
    public override void Init(Vector2 direction, bool facingRight)
    {
        base.Init(
/// <file>
/// <summary>
/// StandartBullet.cs - Specific implementation for player's default weapon
/// </summary>
/// </file>

using Godot;
using System;

/// <summary>
/// Concrete implementation of a standard player projectile.
/// </summary>
/// <remarks>
/// Inherits from <see cref="BulletInheritance"/>.
/// Automatically configures itself to identify as a "player_bullet" 
/// and target "enemy" objects.
/// </remarks>
public partial class StandartBullet : BulletInheritance
{
    /// <summary>
    /// Configures default properties for the player's bullet on scene entry.
    /// </summary>
    /// <remarks>
    /// Overrides base settings:
    /// <list type="bullet">
    /// <item>Self Group: "player_bullet"</item>
    /// <item>Opposite Group: "enemy"</item>
    /// <item>Speed: 1000f</item>
    /// </list>
    /// </remarks>
    public override void _Ready()
    {
        base._Ready();
        selfGroup = "player_bullet";
        oppositeGroup = "enemy";
        speed = 1000f;
    }

    /// <summary>
    /// Initializes the bullet logic and explicitly confirms group membership.
    /// </summary>
    /// <param name="direction">The calculated flight vector.</param>
    /// <param name="facingRight">The facing direction of the shooter.</param>
    public override void Init(Vector2 direction, bool facingRight)
    {
        base.Init(direction, facingRight);
        AddToGroup("player_bullet");
    }
}
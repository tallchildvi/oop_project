/// <file>
/// <summary>
/// BulletSpawner.cs - Helper class for instantiating and launching projectiles
/// </summary>
/// </file>

using Godot;
using System;

/// <summary>
/// Handles the instantiation and initialization of bullet scenes.
/// </summary>
/// <remarks>
/// Encapsulates the logic for spawning bullets, calculating their initial direction
/// (either towards a target or straight ahead), and adding them to the scene tree.
/// </remarks>
public class BulletSpawner
{
    /// <summary>The node responsible for spawning (origin point).</summary>
    private Node2D owner;
    
    /// <summary>The bullet prefab to instantiate.</summary>
    private PackedScene bulletScene;
    
    /// <summary>Optional group to assign to spawned bullets.</summary>
    private string bulletGroup;

    /// <summary>
    /// Configures the spawner.
    /// </summary>
    /// <param name="owner">The entity that fires the bullet (e.g., Enemy or Player).</param>
    /// <param name="bulletScene">The PackedScene resource of the bullet.</param>
    /// <param name="bulletGroup">Optional group name (e.g., "enemy_bullet"). Default is empty.</param>
    public BulletSpawner(Node2D owner, PackedScene bulletScene, string bulletGroup = "")
    {
        this.owner = owner;
        this.bulletScene = bulletScene;
        this.bulletGroup = bulletGroup ?? "";
    }

    /// <summary>
    /// Instantiates a bullet and directs it towards a target or direction.
    /// </summary>
    /// <param name="player">The target node to aim at. If null, shoots straight.</param>
    /// <param name="facingRight">Direction flag used if no target is provided.</param>
    /// <remarks>
    /// <para>
    /// If <paramref name="player"/> is provided, calculates a normalized vector from owner to player.
    /// Otherwise, uses <paramref name="facingRight"/> to determine horizontal direction (Vector2.Right/Left).
    /// </para>
    /// <para>
    /// Attempts to add the bullet to the current scene root to avoid it moving with the parent.
    /// </para>
    /// </remarks>
    public void SpawnBullet(Node2D player, bool facingRight)
    {
        if (bulletScene == null || owner == null) return;

        var node = bulletScene.Instantiate();
        if (node == null) return;

        // Configure BulletInheritance if the type matches
        if (node is BulletInheritance bullet)
        {
            bullet.GlobalPosition = owner.GlobalPosition;
            Vector2 direction = Vector2.Right;
            
            // Calculate direction: Aim at player OR shoot straight
            if (player != null)
                direction = (player.GlobalPosition - owner.GlobalPosition).Normalized();
            else
            {
                GD.Print("[BulletSpawner] player is null");
                direction = facingRight ? Vector2.Right : Vector2.Left;
            }
            GD.Print($"[BulletSpawner] direction: {direction}");

            bullet.Init(direction, facingRight);

            if (!string.IsNullOrEmpty(bulletGroup))
                bullet.AddToGroup(bulletGroup);
            
            // Add to scene tree (prefer Root to decouple movement)
            var root = owner.GetTree().CurrentScene;
            if (root != null)
                root.AddChild(bullet);
            else
                owner.GetParent()?.AddChild(bullet);
            return;
        }

        // Fallback for non-BulletInheritance nodes
        var rootFallback = owner.GetTree().CurrentScene;
        if (rootFallback != null)
            rootFallback.AddChild(node);
        else
            owner.GetParent()?.AddChild(node);

        GD.PrintErr("[BulletSpawner] Spawned bullet instance does not inherit BulletInheritance. Ensure prefab matches expected type.");
    }
}
/// <file>
/// <summary>
/// EnemyPool.cs - Implements an object pooling system for <c>BaseEnemy</c> instances.
/// </summary>
/// <remarks>
/// This pool significantly improves performance by recycling enemy nodes instead of
/// constantly instantiating and freeing them during gameplay (e.g., during waves).
/// It uses a <see cref="Queue{T}"/> to manage available instances.
/// </remarks>
/// </file>
using Godot;
using System.Collections.Generic;

/// <summary>
/// Manages a pool of enemy objects (<c>BaseEnemy</c>) for efficient reuse.
/// This prevents runtime overhead associated with frequent <see cref="PackedScene.Instantiate"/> and <see cref="Node.QueueFree"/> calls.
/// </summary>
public partial class EnemyPool : Node
{
    /// <summary>
    /// The <see cref="PackedScene"/> template used to create new enemy instances when the pool is empty.
    /// </summary>
    private PackedScene _enemyScene;

    /// <summary>
    /// The queue used to store and manage the reusable, inactive enemy instances.
    /// </summary>
    private Queue<BaseEnemy> _pool = new Queue<BaseEnemy>();

    /// <summary>
    /// The desired initial number of enemies to pre-instantiate (though pre-instantiation is deferred or handled externally).
    /// </summary>
    private int _initialSize = 10;

    /// <summary>
    /// The parent node under which newly created or retrieved enemies will be added/re-parented when they are activated.
    /// </summary>
    private Node _spawnParent;

    /// <summary>
    /// Initializes the EnemyPool manager with necessary resources and settings.
    /// This method should be called immediately after the pool node is added to the scene tree.
    /// </summary>
    /// <param name="enemyScene">The scene to use for instantiating enemies.</param>
    /// <param name="spawnParent">The node where enemies should be added as children when retrieved.</param>
    /// <param name="initialSize">The initial capacity/size of the pool (currently only used to store the value).</param>
    public void Initialize(PackedScene enemyScene, Node spawnParent, int initialSize = 10)
    {
        _enemyScene = enemyScene;
        _spawnParent = spawnParent;
        // Ensures the initial size is at least 1
        _initialSize = Mathf.Max(1, initialSize);
    }

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// Currently used for logging, noting that object pre-creation is handled elsewhere.
    /// </summary>
    public override void _Ready()
    {
        GD.Print($"[EnemyPool] _Ready called. Pool initialized.");
        // Note: Pre-instantiation is deliberately skipped in _Ready.
    }

    /// <summary>
    /// Retrieves an enemy instance from the pool. If the pool is empty, a new enemy is created.
    /// </summary>
    /// <returns>An activated <c>BaseEnemy</c> instance ready for use, or <c>null</c> if creation failed.</returns>
    public BaseEnemy GetEnemy()
    {
        GD.Print($"[EnemyPool] GetEnemy called. Pool count: {_pool.Count}");

        BaseEnemy enemy;
        if (_pool.Count > 0)
        {
            // Reuse an existing enemy from the pool
            enemy = _pool.Dequeue();
            GD.Print("[EnemyPool] Got enemy from pool");
        }
        else
        {
            // Create a new enemy if the pool is empty (dynamic sizing)
            enemy = CreateNewEnemy();
            GD.Print("[EnemyPool] Created new enemy");
        }

        if (enemy == null)
        {
            GD.PrintErr("[EnemyPool] Enemy is null!");
            return null;
        }

        // Prepare the enemy for use (e.g., enable processing, set initial position)
        // Assumes BaseEnemy has an Activate method
        enemy.Activate(Vector2.Zero);
        GD.Print($"[EnemyPool] Enemy activated. Visible: {enemy.Visible}, Position: {enemy.Position}");
        return enemy;
    }

    /// <summary>
    /// Instantiates a new enemy from the <see cref="_enemyScene"/> and adds it to the designated spawn parent.
    /// </summary>
    /// <returns>A newly created <c>BaseEnemy</c> instance, initialized to an inactive state, or <c>null</c> on failure.</returns>
    private BaseEnemy CreateNewEnemy()
    {
        var node = _enemyScene.Instantiate();
        if (node == null)
        {
            GD.PrintErr("[EnemyPool] Instantiate returned null");
            return null;
        }

        var enemy = node as BaseEnemy;
        if (enemy == null)
        {
            GD.PrintErr("[EnemyPool] enemy scene root is not BaseEnemy");
            node.QueueFree();
            return null;
        }

        // Set the initial state before adding to the scene
        enemy.Visible = false;
        enemy.ProcessMode = ProcessModeEnum.Disabled;

        // Add the new enemy to the spawn parent using CallDeferred to ensure safe scene modification
        _spawnParent.CallDeferred("add_child", enemy);

        return enemy;
    }


    /// <summary>
    /// Returns an enemy instance back to the pool, resetting its state and making it inactive.
    /// </summary>
    /// <param name="enemy">The <c>BaseEnemy</c> instance to return to the pool.</param>
    public void ReturnEnemy(BaseEnemy enemy)
    {
        enemy.RemoveFromGroup("enemy");
        if (enemy == null || !IsInstanceValid(enemy)) return;

        // Reset the enemy to its initial, inactive state
        // Assumes BaseEnemy has a ResetState method
        enemy.ResetState();
        enemy.Visible = false;

        // Enqueue the enemy for later reuse
        _pool.Enqueue(enemy);
    }
}
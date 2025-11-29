using Godot;
using System.Collections.Generic;

public partial class EnemyPool : Node
{
	private PackedScene _enemyScene;
	private Queue<BaseEnemy> _pool = new Queue<BaseEnemy>();
	private int _initialSize = 10;
	private Node _spawnParent; 

	public void Initialize(PackedScene enemyScene, Node spawnParent, int initialSize = 10)
	{
		_enemyScene = enemyScene;
		_spawnParent = spawnParent;
		_initialSize = Mathf.Max(1, initialSize);
	}

	public override void _Ready()
{
	GD.Print($"[EnemyPool] _Ready called. Pool initialized.");
	// Не створюємо enemies тут!
}

	public BaseEnemy GetEnemy()
	{
		GD.Print($"[EnemyPool] GetEnemy called. Pool count: {_pool.Count}");
		
		BaseEnemy enemy;
		if (_pool.Count > 0)
		{
			enemy = _pool.Dequeue();
			GD.Print("[EnemyPool] Got enemy from pool");
		}
		else
		{
			enemy = CreateNewEnemy();
			GD.Print("[EnemyPool] Created new enemy");
		}
		
		if (enemy == null)
		{
			GD.PrintErr("[EnemyPool] Enemy is null!");
			return null;
		}
		
		enemy.Activate(Vector2.Zero);
		GD.Print($"[EnemyPool] Enemy activated. Visible: {enemy.Visible}, Position: {enemy.Position}");
		return enemy;
	}

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
		
		// Встановлюємо стан ДО додавання в сцену
		enemy.Visible = false;
		enemy.ProcessMode = ProcessModeEnum.Disabled;
		
		// Використовуємо CallDeferred
		_spawnParent.CallDeferred("add_child", enemy);
		
		return enemy;
	}
	

	public void ReturnEnemy(BaseEnemy enemy)
	{
		enemy.RemoveFromGroup("enemy");
		if (enemy == null || !IsInstanceValid(enemy)) return;
		enemy.ResetState();
		enemy.Visible = false;
		_pool.Enqueue(enemy);
	}
}

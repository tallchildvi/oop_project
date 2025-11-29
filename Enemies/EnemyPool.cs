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
		if (_enemyScene == null || _spawnParent == null)
		{
			GD.PrintErr("[EnemyPool] Not initialized! Call Initialize() before adding to scene.");
			return;
		}

		for (int i = 0; i < _initialSize; i++)
		{
			var enemy = CreateNewEnemy();
			if (enemy == null) continue;
			enemy.Visible = false;
			enemy.ResetState();
			_pool.Enqueue(enemy);
		}
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

		//_spawnParent.CallDeferred(nameof(AddChild), enemy);
		_spawnParent.CallDeferred("add_child", enemy);

		enemy.Visible = false;
		enemy.ResetState();
		return enemy;
	}

	public BaseEnemy GetEnemy()
	{
		BaseEnemy enemy;
		if (_pool.Count > 0)
			enemy = _pool.Dequeue();
		else
			enemy = CreateNewEnemy();

		if (enemy == null) return null;

		enemy.Activate(Vector2.Zero);
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

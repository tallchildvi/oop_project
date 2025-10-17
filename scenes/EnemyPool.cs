using Godot;
using System.Collections.Generic;

public partial class EnemyPool : Node
{
	private PackedScene _enemyScene;
	private Queue<Enemy> _pool = new Queue<Enemy>();
	private int _initialSize = 10;
	private Node _spawnParent; // Куди додавати ворогів у сцені (наприклад, Level)

	public void Initialize(PackedScene enemyScene, Node spawnParent, int initialSize = 10)
	{
		_enemyScene = enemyScene;
		_spawnParent = spawnParent;
		_initialSize = initialSize;
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
			enemy.Visible = false;
			_pool.Enqueue(enemy);
		}
	}

	private Enemy CreateNewEnemy()
	{
		var enemy = _enemyScene.Instantiate<Enemy>();
		_spawnParent.CallDeferred("add_child", enemy);
		enemy.Visible = false;
		return enemy;
	}

	public Enemy GetEnemy()
	{
		Enemy enemy = _pool.Count > 0 ? _pool.Dequeue() : CreateNewEnemy();
		enemy.Visible = true;
		enemy.ResetState();
		return enemy;
	}

	public void ReturnEnemy(Enemy enemy)
	{
		if (enemy == null || !IsInstanceValid(enemy)) return;

		enemy.Visible = false;
		enemy.ResetState();
		_pool.Enqueue(enemy);
	}
}

using Godot;
using System;
using System.Collections.Generic;

public class EnemyPool : Node
{
	private PackedScene _enemyScene;
	private Queue<Enemy> _pool = new Queue<Enemy>();

	private int _initialSize = 10;

	public EnemyPool(PackedScene enemyScene, int initialSize = 10)
	{
		_enemyScene = enemyScene;
		_initialSize = initialSize;
	}

	public override void _Ready()
	{
		for (int i = 0; i < _initialSize; i++)
		{
			var enemy = CreateNewEnemy();
			enemy.Visible = false;
			_pool.Enqueue(enemy);
		}
	}

	private Enemy CreateNewEnemy()
	{
		Enemy enemy = _enemyScene.Instantiate<Enemy>();
		AddChild(enemy);
		enemy.Visible = false;
		return enemy;
	}

	public Enemy GetEnemy()
	{
		Enemy enemy;

		if (_pool.Count > 0)
			enemy = _pool.Dequeue();
		else
			enemy = CreateNewEnemy();

		enemy.Visible = true;
		enemy.ResetState();
		return enemy;
	}

	public void ReturnEnemy(Enemy enemy)
	{
		enemy.Visible = false;
		enemy.ResetState();
		_pool.Enqueue(enemy);
	}
}

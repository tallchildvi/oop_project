using Godot;
using System;

public partial class EnemyManager : Node
{
	public static EnemyManager Instance { get; private set; }

	private List<Node> activeEnemies = new List<Node>();
	private Queue<Node> enemyPool = new Queue<Node>();

	public override void _Ready()
	{
		Instance = this;
	}

	public void SpawnWave(int waveNumber)
	{
		// наприклад, дані хвиль можна взяти з конфігурації рівня
		for (int i = 0; i < waveNumber * 5; i++)
		{
			var enemy = GetEnemyFromPool();
			enemy.Position = GetSpawnPoint();
			GetTree().CurrentScene.AddChild(enemy);
			activeEnemies.Add(enemy);
		}
	}

	private Node GetEnemyFromPool()
	{
		if (enemyPool.Count > 0)
			return enemyPool.Dequeue();

		var scene = GD.Load<PackedScene>("res://Enemies/BasicEnemy.tscn");
		return scene.Instantiate<Node>();
	}

	public void RecycleEnemy(Node enemy)
	{
		enemy.Visible = false;
		activeEnemies.Remove(enemy);
		enemyPool.Enqueue(enemy);
	}
}

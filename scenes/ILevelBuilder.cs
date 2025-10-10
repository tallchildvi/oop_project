using Godot;
using System;

public interface ILeveluilder : Node
{
	void BuildMap(string mapName);
	void BuildPlayer(string characterId);
	void BuildEnemies(int difficulty);
	void BuildUI();
	Level GetResult();
}

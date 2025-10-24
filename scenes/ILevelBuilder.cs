using Godot;
using System;

public interface ILevelBuilder
{
	void BuildMap(string mapName);
	void BuildPlayer(string characterId);
	void BuildEnemies(int difficulty);
	void BuildUI();
	Level GetResult();
}
 

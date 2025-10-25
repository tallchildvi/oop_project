using Godot;
using System;

public interface ILevelBuilder
{
	void BuildMap(string mapName);
	void BuildPlayer(string characterId);
	//void BuildEnemies(int difficulty);
	void BuildEnemies();
	void BuildUI();
	Level GetResult();
}
 

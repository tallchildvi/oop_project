using Godot;
using System;

public partial class CharacterManager : Node2D
{
	public Node2D Player;

	
	private PackedScene characterBase;
	private PackedScene characterTombstoneBase;
	private MainCharacter character;
	private Tombstone characterTombstone;
	private float hp = 100;
	private float bullet_counter = 5;
	private bool exists = false;
	
	//public override void _Ready()
	//{
		//characterBase = ResourceLoader.Load<PackedScene>("res://characters/Atlas/main_character.tscn");
		//SpawnCharacter();
	//}

	public override void _Process(double delta)
	{
		if (Input.IsKeyPressed(Key.Q))
		{
			characterBase = ResourceLoader.Load<PackedScene>("res://characters/Atlas/main_character.tscn");
			SpawnCharacter();
		}
	}
		
	private void SpawnCharacter()
	{
		if(!exists){
			character = (MainCharacter)characterBase.Instantiate();
			hp = 100;
			character.GlobalPosition = GlobalPosition;
			character.init(200f,400f,0.5f,0.4f,0.5f);
			AddChild(character);
			exists = true;
		}
	}

	public void TakeDamage(float damage)
	{
		hp -= damage;
		GD.Print(hp);
		if (hp <= 0)
		{
			GD.Print("Character died!");
			if (character != null && IsInstanceValid(character)){
				characterTombstoneBase = ResourceLoader.Load<PackedScene>("res://characters/tombstone.tscn");
				characterTombstone = (Tombstone)characterTombstoneBase.Instantiate();
				characterTombstone.GlobalPosition = character.GlobalPosition;
				AddChild(characterTombstone);
				character.QueueFree();
				character = null;
				exists = false;
			}
		}
	}
	
	public void Reload(float bullets){
		bullet_counter += bullets;
	}
	
	public bool bullet_manage(){
		if (bullet_counter > 0){
			bullet_counter -= 1;
			GD.Print(bullet_counter);
			return true;
		}
		return false;
	}
}

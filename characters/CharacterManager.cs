using Godot;
using System;

public partial class CharacterManager : Node2D
{
	private PackedScene characterBase;
	private PackedScene characterTombstoneBase;
	private MainCharacter character;
	private Tombstone characterTombstone;
	private float hp = 100;
	private float bullet_counter = 5;
	private bool exists = false;
	public float currency = 0;
	public bool allow_shop = false;
  	public bool shop_exists = false;
	
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
		if (Input.IsKeyPressed(Key.M) && allow_shop && !shop_exists){
			PackedScene shopBase = ResourceLoader.Load<PackedScene>("res://characters/trader/trader_menu.tscn");
			TraderMenu menu = shopBase.Instantiate() as TraderMenu;
			if (menu != null)
			{
				menu.manager = this;
				AddChild(menu);
				shop_exists = true;
	  		}
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
	  public void addMoney(float money){
	currency += money;
  }

  public void luck(int buff){
	switch (buff){
	  case 0:
		Reload(2);
		break;
	  case 1:
		Reload(5);
		break;
	  case 2:
		Reload(10);
		break;
	  case 3:
		GD.Print("Scooter added:");
		if (character == null || !IsInstanceValid(character))
		{
		  GD.PrintErr("No valid character to attach scooter to!");
		  return;
		}
		var scooterBase = (PackedScene)ResourceLoader.Load("res://items/scooter.tscn");
		Scooter scooter = (Scooter)scooterBase.Instantiate();
		scooter.manager = this;
		character.AddChild(scooter);
		GD.Print("Scooter added:", scooter.GetPath());
		break;
	}
  }
}

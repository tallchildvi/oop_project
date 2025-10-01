using Godot;
using System;

public partial class MainCharacter : Sprite2D
{
	private float speed = 200f;
	private Vector2 movement = Vector2.Zero;
	private Sprite2D weapon;
	private Sprite2D characterSprite;
	private Vector2 originalWeaponScale;
	private Vector2 originalCharacterScale;
	private Vector2 dashDirection;
	private bool facingRight = true;
	private bool in_dash = false;
	private float dashSpeed = 600f;
	private float dashTime = 0.3f;
	private float dashTimer = 0f;
	private float dashReloadTime = 4f;
	private float dashReloadTimer = 0f;
	private float bulletReloadTime = 0.5f;
	private float bulletReloadTimer = 0f;
	private PackedScene bulletBase;

	
	public override void _Ready()
	{
		weapon = GetNode<Sprite2D>("weapon");
		characterSprite = GetNode<Sprite2D>("Character");
		bulletBase = ResourceLoader.Load<PackedScene>("res://characters/standart_bullet.tscn");
		originalWeaponScale = weapon.Scale;
		originalCharacterScale = characterSprite.Scale;
		characterSprite.FlipH = false;
		weapon.Scale = new Vector2(Mathf.Abs(originalWeaponScale.X), originalWeaponScale.Y);
		facingRight = true;
	}
	
	private void StartDash(Vector2 direction)
	{
		in_dash = true;
		dashDirection = direction.Normalized();
		dashTimer = dashTime;
	}
	
	public override void _Process(double delta)
	{
		movement = Vector2.Zero;
		if(!in_dash){
			if (Input.IsKeyPressed(Key.W))
				movement.Y -= 1;
			if (Input.IsKeyPressed(Key.S))
				movement.Y += 1;
			if (Input.IsKeyPressed(Key.A))
				movement.X -= 1;
			if (Input.IsKeyPressed(Key.D))
				movement.X += 1;
			if (Input.IsKeyPressed(Key.Shift) && dashReloadTimer <= 0f && movement != Vector2.Zero)
			 Dash();
			if (Input.IsKeyPressed(Key.Space)) {
				if(bulletReloadTimer <= 0) Shoot();
			}
		}
		if (in_dash)
		{
			Position += dashDirection * dashSpeed * (float)delta;
			dashTimer -= (float)delta;

			if (dashTimer <= 0f){
				in_dash = false;
				dashReloadTimer = dashReloadTime;
			} 
		}
		else if (movement != Vector2.Zero)
		{	
			if(dashReloadTimer >= 0f) dashReloadTimer -= (float)delta;
			movement = movement.Normalized();
			Position += movement * speed * (float)delta;

			float angle = Mathf.Atan2(movement.Y, movement.X);
			weapon.RotationDegrees = Mathf.RadToDeg(angle);

			if (movement.X < 0 && facingRight)
			{
				facingRight = false;
				characterSprite.FlipH = true;
				weapon.Scale = new Vector2(Mathf.Abs(originalWeaponScale.X), -Mathf.Abs(originalWeaponScale.Y));
			}
			else if (movement.X > 0 && !facingRight)
			{
				facingRight = true;
				characterSprite.FlipH = false;
				weapon.Scale = new Vector2(Mathf.Abs(originalWeaponScale.X), originalWeaponScale.Y);
			}
			
		}
		if(bulletReloadTimer >= 0f) bulletReloadTimer -= (float)delta;
	}
	
	public void Dash(){
		in_dash = true;
		dashDirection = movement.Normalized();
		dashTimer = dashTime;
	}
	
	public void Shoot()
	{
		if (bulletBase == null)
		{
			GD.PrintErr("Bullet scene not found!");
			return;
		}

		var bullet = (StandartBullet)bulletBase.Instantiate();
		var spawnMarker = GetNode<Marker2D>("weapon/bullet_spawn");
		Vector2 spawnPos = spawnMarker.GlobalPosition;

		Node parent;
		if (GetTree().CurrentScene != this)
		{
			if (GetTree().CurrentScene.HasNode("Bullets"))
				parent = GetTree().CurrentScene.GetNode("Bullets");
			else
				parent = GetParent();
		}
		else
		{
			parent = this;
		}

		parent.AddChild(bullet);
		bullet.GlobalPosition = spawnPos;
		bullet.Init(movement, facingRight);
		bulletReloadTimer = bulletReloadTime;
	}
}

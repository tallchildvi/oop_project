//using Godot;
//using System;
//
//public partial class BaseCharacter : Area2D
//{
	//[Export] protected float speed = 200f;
	//[Export] protected float dashSpeed = 400f;
	//[Export] protected float dashTime = 0.2f;
	//[Export] protected float dashReloadTime = 1f;
	//[Export] protected float bulletReloadTime = 0.4f;
	//[Export] protected int maxHealth = 5;
//
	//protected Vector2 movement = Vector2.Zero;
	//protected Vector2 dashDirection;
	//protected bool inDash = false;
	//protected float dashTimer = 0f;
	//protected float dashReloadTimer = 0f;
	//protected float bulletReloadTimer = 0f;
	//protected bool facingRight = true;
	//protected int health;
//
	//protected PackedScene bulletBase;
	//protected bool control = true;
//
	//public override void _Ready()
	//{
		//AddToGroup("player");
		//bulletBase = ResourceLoader.Load<PackedScene>("res://Characters/Atlas/standart_bullet.tscn");
		//health = maxHealth;
//
		//// Гарантуємо виклик Process() й PhysicsProcess()
		//ProcessMode = ProcessModeEnum.Always;
		//SetPhysicsProcess(true);
	//}
//
	//// таймери — у звичайному процесі
	//public override void _Process(double delta)
	//{
		//HandleReloadTimers(delta);
	//}
//
	//// Рух у фізичному процесі — стабільніше
	//public override void _PhysicsProcess(double delta)
	//{
		//if (!control)
			//return;
//
		//HandleMovementPhysics(delta);
		//HandleDashPhysics(delta);
		//HandleIdleState();
	//}
//
	//// ====== РУХ (physics) ======
	//protected virtual void HandleMovementPhysics(double delta)
	//{
		//if (inDash) return;
//
		//Vector2 input = Vector2.Zero;
		//if (Input.IsKeyPressed(Key.W)) input.Y -= 1;
		//if (Input.IsKeyPressed(Key.S)) input.Y += 1;
		//if (Input.IsKeyPressed(Key.A)) input.X -= 1;
		//if (Input.IsKeyPressed(Key.D)) input.X += 1;
//
		//movement = input;
//
		//if (movement != Vector2.Zero)
		//{
			//movement = movement.Normalized();
//
			//Vector2 oldPos = GlobalPosition;
			//Vector2 target = oldPos + movement * speed * (float)delta;
			//GlobalPosition = target; // використовуємо GlobalPosition
//
			//// Діагностика: якщо позиція не змінилась — вивести повідомлення з контекстом
			//if (GlobalPosition == oldPos)
			//{
				//GD.PrintErr($"[BaseCharacter] Movement attempted but GlobalPosition unchanged. Parent: {GetParent()?.GetType().Name} Path: {GetPath()}");
			//}
//
			//OnMove(movement);
		//}
//
		//// dash і shoot мають реагувати на input — але викликаємо їх тут
		//if (Input.IsKeyPressed(Key.Shift) && dashReloadTimer <= 0f && movement != Vector2.Zero)
			//StartDash(movement);
//
		//if (Input.IsKeyPressed(Key.J))
			//TryShoot();
	//}
//
	//protected virtual void OnMove(Vector2 dir) { }
	//protected virtual void OnIdle() { }
	//protected virtual void OnDashStart() { }
	//protected virtual void OnDashEnd() { }
//
	//protected virtual void HandleDashPhysics(double delta)
	//{
		//if (!inDash) return;
//
		//Vector2 oldPos = GlobalPosition;
		//GlobalPosition += dashDirection * dashSpeed * (float)delta;
		//dashTimer -= (float)delta;
//
		//if (dashTimer <= 0f)
		//{
			//inDash = false;
			//dashReloadTimer = dashReloadTime;
			//OnDashEnd();
		//}
//
		//if (GlobalPosition == oldPos)
			//GD.PrintErr($"[BaseCharacter] Dash moved 0 units. Parent: {GetParent()?.GetType().Name} Path: {GetPath()}");
	//}
//
	//protected virtual void HandleReloadTimers(double delta)
	//{
		//if (dashReloadTimer > 0f) dashReloadTimer -= (float)delta;
		//if (bulletReloadTimer > 0f) bulletReloadTimer -= (float)delta;
	//}
//
	//protected virtual void HandleIdleState()
	//{
		//if (!inDash && movement == Vector2.Zero)
			//OnIdle();
	//}
//
	//protected virtual void StartDash(Vector2 direction) 
	//{
		//inDash = true;
		//dashDirection = direction.Normalized();
		//dashTimer = dashTime;
		//OnDashStart();
	//}
//
	//protected virtual void TryShoot()
	//{
		//if (bulletReloadTimer > 0f || bulletBase == null)
			//return;
//
		//Shoot();
		//bulletReloadTimer = bulletReloadTime;
	//}
//
	//protected virtual void Shoot(){}
//
	//public virtual void TakeDamage(int damage)
	//{
		//if (health - damage > 0)
		//{
			//health -= damage;
			//GD.Print("[Player] take damage");
		//}
		//else
		//{
			//GD.Print("[Player] Died");
			//health = 0;
			//EventManager.TriggerEvent("GAME_OVER", this);
		//}
	//}
//
	//public void EnableControl() => control = true;
	//public void DisableControl() => control = false;
//}

//using Godot;
//using System;
//
//public partial class BaseCharacter : Area2D
//{
	//[Export] protected float speed = 200f;
	//[Export] protected float dashSpeed = 400f;
	//[Export] protected float dashTime = 0.2f;
	//[Export] protected float dashReloadTime = 1f;
	//[Export] protected float bulletReloadTime = 0.4f;
	//[Export] protected int maxHealth = 5;
	//[Export] protected PackedScene bulletBase; // можна задати в інспекторі
//
	//protected Vector2 movement = Vector2.Zero; // нормалізований напрям руху (від вводу)
	//protected Vector2 dashDirection = Vector2.Zero;
	//protected bool inDash = false;
	//protected float dashTimer = 0f;
	//protected float dashReloadTimer = 0f;
	//protected float bulletReloadTimer = 0f;
	//protected bool facingRight = true;
	//protected int health;
	//protected bool control = true;
//
	//private bool debugMoveTest = true; // тимчасово: якщо true — автоматично рухає node вправо для тесту
	//private bool firstReadyLog = true;
//
	//public override void _Ready()
	//{
		//AddToGroup("player");
//
		//if (bulletBase == null)
			//bulletBase = ResourceLoader.Load<PackedScene>("res://Characters/Atlas/standart_bullet.tscn");
//
		//health = maxHealth;
		//SetPhysicsProcess(true);
//
		//// Простий Ready-log (виводиться один раз)
		//if (firstReadyLog)
		//{
			//firstReadyLog = false;
			//GD.Print($"[DBG Ready] Path: {GetPath()} Parent: {GetParent()?.GetPath()} TreePaused: {GetTree().Paused}");
		//}
//
		//// Короткий одноразовий рух-тест (перемістить трохи вліво/вправо на старті)
		//Vector2 tryMove = new Vector2(16, 0);
		//Position += tryMove;
		//GD.Print($"[DBG Ready] Position after one-time offset: {Position}");
	//}
//
	//protected void ReadInput()
	//{
		//Vector2 input = Vector2.Zero;
		//if (Input.IsKeyPressed(Key.W)) input.Y -= 1;
		//if (Input.IsKeyPressed(Key.S)) input.Y += 1;
		//if (Input.IsKeyPressed(Key.A)) input.X -= 1;
		//if (Input.IsKeyPressed(Key.D)) input.X += 1;
//
		//movement = input == Vector2.Zero ? Vector2.Zero : input.Normalized();
//
		//// debug: покажемо що зчитали
		//GD.Print($"[DBG Input] movement={movement} inDash={inDash} control={control}");
//
		//if (Input.IsKeyPressed(Key.Shift) && dashReloadTimer <= 0f && movement != Vector2.Zero && !inDash)
			//StartDash(movement);
//
		//if (Input.IsKeyPressed(Key.J))
			//TryShoot();
	//}
//
	//protected  void HandleMovementPhysics(float delta)
	//{
		//if (inDash) return;
//
		//// Якщо є ввід — рухаємося за ним
		//if (movement != Vector2.Zero)
		//{
			//Vector2 oldPos = Position;
			//Vector2 deltaMove = movement * speed * (float)delta;
			//Position += deltaMove;
			//GD.Print($"[DBG Move] moved by {deltaMove} -> pos {Position}");
			//OnMove(movement);
			//return;
		//}
//
		//// Нема вводу — виконати простий тестовий рух (щоб перевірити, чи взагалі змінюється Position)
		//if (debugMoveTest)
		//{
			//Vector2 testDelta = Vector2.Right * (50f * (float)delta); // 50 px/sec вправо
			//Position += testDelta;
			//GD.Print($"[DBG AutoMove] test moved {testDelta} -> pos {Position}");
			//OnMove(Vector2.Right);
		//}
	//}
//
	//protected override void HandleDashPhysics(float delta)
	//{
		//if (!inDash) return;
//
		//Vector2 deltaMove = dashDirection * dashSpeed * (float)delta;
		//Position += deltaMove;
		//dashTimer -= (float)delta;
		//GD.Print($"[DBG Dash] dash moved {deltaMove} -> pos {Position}");
//
		//if (dashTimer <= 0f)
		//{
			//inDash = false;
			//dashReloadTimer = dashReloadTime;
			//OnDashEnd();
		//}
	//}
//
//
//
	//protected virtual void StartDash(Vector2 direction)
	//{
		//inDash = true;
		//dashDirection = direction.Normalized();
		//dashTimer = dashTime;
		//// Щоб уникнути конфлікту звичайного руху під час dash
		//movement = Vector2.Zero;
		//OnDashStart();
	//}
//
	//// ---------- Shooting ----------
	//protected virtual void TryShoot()
	//{
		//if (bulletReloadTimer > 0f || bulletBase == null) return;
//
		//Shoot();
		//bulletReloadTimer = bulletReloadTime;
		//OnShoot();
	//}
//
	//// Єдиний метод, що створює кулю — підкласи повинні лише повертати позицію спавну якщо треба
	//protected virtual void Shoot()
	//{
		//if (bulletBase == null)
		//{
			//GD.PrintErr("[BaseCharacter] bulletBase is null.");
			//return;
		//}
//
		//var obj = bulletBase.Instantiate();
		//if (obj is StandartBullet bullet)
		//{
			//Vector2 spawn = GetBulletSpawnPosition();
			//bullet.GlobalPosition = spawn;
//
			//// Якщо гравець стоїть — стріляємо в бік, куди дивимось
			//Vector2 dir = movement != Vector2.Zero ? movement : (facingRight ? Vector2.Right : Vector2.Left);
			//bullet.Init(dir, facingRight);
//
			//var root = GetTree().CurrentScene;
			//if (root != null) root.AddChild(bullet);
			//else GetParent()?.AddChild(bullet);
		//}
		//else
		//{
			//GD.PrintErr("[BaseCharacter] instantiated bullet is not StandartBullet");
		//}
	//}
//
	//// Повертає позицію спавну кулі (базова — позиція персонажа). Atlas перевизначить.
	//protected virtual Vector2 GetBulletSpawnPosition()
	//{
		//return GlobalPosition;
	//}
//
	//// ---------- Timers & Idle ----------
	//protected virtual void HandleReloadTimers(float delta)
	//{
		//if (dashReloadTimer > 0f) dashReloadTimer = Math.Max(0f, dashReloadTimer - delta);
		//if (bulletReloadTimer > 0f) bulletReloadTimer = Math.Max(0f, bulletReloadTimer - delta);
	//}
//
	//protected virtual void HandleIdleState()
	//{
		//if (!inDash && movement == Vector2.Zero)
			//OnIdle();
	//}
//
	//// ---------- Hooks ----------
	//protected virtual void OnMove(Vector2 dir) { }
	//protected virtual void OnIdle() { }
	//protected virtual void OnDashStart() { }
	//protected virtual void OnDashEnd() { }
	//protected virtual void OnShoot() { }
//
	//// ---------- Health ----------
	//public virtual void TakeDamage(int damage)
	//{
		//health -= damage;
		//if (health > 0)
			//GD.Print("[Player] take damage");
		//else
		//{
			//health = 0;
			//GD.Print("[Player] Died");
			//EventManager.TriggerEvent("GAME_OVER", this);
		//}
	//}
//
	//public void EnableControl() => control = true;
	//public void DisableControl() => control = false;
//}

using Godot;
using System;

public partial class BaseCharacter : Area2D
{
	//protected float speed = 200f;
	public float speed = 200f;
	protected float dashSpeed = 400f;
	protected float dashTime = 0.2f;
	protected float dashReloadTime = 1f;
	protected float bulletReloadTime = 0.4f;
	protected int maxHealth = 5;

	protected int maxAmmo = 10;
	protected int ammo = 0;

	protected PackedScene bulletBase; // можна задати в інспекторі

	// рух / dash / таймери
	protected Vector2 movement = Vector2.Zero;
	protected Vector2 dashDirection = Vector2.Zero;
	protected bool inDash = false;
	protected float dashTimer = 0f;
	protected float dashReloadTimer = 0f;
	protected float bulletReloadTimer = 0f;
	protected bool facingRight = true;
	protected int health;
	protected bool control = true;

	public override void _Ready()
	{
		AddToGroup("player");
		if (bulletBase == null)
			bulletBase = ResourceLoader.Load<PackedScene>("res://characters/Atlas/standart_bullet.tscn");

		health = maxHealth;
		ammo = maxAmmo;
	}

	public void Init(float speed = 200f, float dashSpeed = 400f, float dashTime = 0.2f, 
		float dashReloadTime = 1f, float bulletReloadTime = 0.4f, int maxAmmo = 10)
	{
		GD.Print("try to init character");
		this.speed = speed;
		this.dashSpeed = dashSpeed;
		this.dashTime = dashTime;
		this.dashReloadTime = dashReloadTime;
		this.bulletReloadTime = bulletReloadTime;
		this.maxAmmo = maxAmmo;
		ammo = Mathf.Clamp(ammo, 0, maxAmmo); 
	}

	public override void _Process(double delta)
	{
		if (!control) return;

		// таймери
		if (dashReloadTimer > 0f) dashReloadTimer = Math.Max(0f, dashReloadTimer - (float)delta);
		if (bulletReloadTimer > 0f) bulletReloadTimer = Math.Max(0f, bulletReloadTimer - (float)delta);

		ReadInput();

		// рух / dash
		if (inDash)
		{
			Position += dashDirection * dashSpeed * (float)delta;
			dashTimer -= (float)delta;
			if (dashTimer <= 0f)
			{
				inDash = false;
				dashReloadTimer = dashReloadTime;
				OnDashEnd();
			}
		}
		else
		{
			if (movement != Vector2.Zero)
			{
				movement = movement.Normalized();
				Position += movement * speed * (float)delta;
				OnMove(movement);
				GD.Print($"Speed: {speed}");
				GD.Print($"Movement: {movement}");
				GD.Print($"Position: {Position}");
				GD.Print($"delta: {delta}");
				GD.Print($"expected change of position: {movement * speed}");
			}
			else
			{
				OnIdle();
			}
		}
	}

	// Просте зчитування вводу (Space = стріляти)
	protected virtual void ReadInput()
	{
		movement = Vector2.Zero;
		if (Input.IsKeyPressed(Key.W)) movement.Y -= 1;
		if (Input.IsKeyPressed(Key.S)) movement.Y += 1;
		if (Input.IsKeyPressed(Key.A)) movement.X -= 1;
		if (Input.IsKeyPressed(Key.D)) movement.X += 1;

		if (Input.IsKeyPressed(Key.Shift) && dashReloadTimer <= 0f && movement != Vector2.Zero && !inDash)
			StartDash(movement);

		if (Input.IsKeyPressed(Key.J))
			TryShoot();
	}

	protected virtual void StartDash(Vector2 direction)
	{
		inDash = true;
		dashDirection = direction.Normalized();
		dashTimer = dashTime;
		movement = Vector2.Zero; // щоб не було конфлікту
		OnDashStart();
	}

	// TryShoot контролює релоади і боєзапас, а потім викликає Shoot() підкласу
	protected virtual void TryShoot()
	{
		// мінімальні перевірки
		if (bulletReloadTimer > 0f) return;
		if (bulletBase == null) return;
		if (ammo <= 0) return;

		// виклик реалізації в Atlas
		if (Shoot())
		{
			bulletReloadTimer = bulletReloadTime;
			ConsumeAmmo(1);
			OnShoot();
		}
	}

	protected virtual bool Shoot()
	{
		return false;
	}

	public int GetAmmo() => ammo;
	public int GetMaxAmmo() => maxAmmo;

	public void Reload(int amount)
	{
		ammo = Mathf.Clamp(ammo + amount, 0, maxAmmo);
	}

	public void SetAmmo(int value)
	{
		ammo = Mathf.Clamp(value, 0, maxAmmo);
	}

	protected void ConsumeAmmo(int amount)
	{
		ammo = Mathf.Clamp(ammo - amount, 0, maxAmmo);
	}

	protected virtual void OnMove(Vector2 dir) { }
	protected virtual void OnIdle() { }
	protected virtual void OnDashStart() { }
	protected virtual void OnDashEnd() { }
	protected virtual void OnShoot() { }

	public virtual void TakeDamage(int damage)
	{
		health -= damage;
		if (health > 0)
			GD.Print("[Player] take damage");
		else
		{
			health = 0;
			GD.Print("[Player] Died");
			EventManager.TriggerEvent("GAME_OVER", this);
		}
	}

	public void EnableControl() => control = true;
	public void DisableControl() => control = false;
}

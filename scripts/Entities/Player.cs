using Godot;

namespace Shootemmono.scripts.Entities;
public partial class Player : CharacterBody2D
{
	[Export]
	public float FireRate {get; set;} = 0.2f;

	[Export]
	public float Speed { get; set; } = 90.0f;

	[Export]
	public float MaxHealth = 100.0f;
	
	[Export]
	public float RegenRate = 2.0f;
	private float _health;

	[Export] 
	public float DashCooldown = 3.0f;

	[Export] public float DashSpeedMultiplier = 3.0f;

	[Export] public float DashDuration = 0.5f;

	private bool _canDash = true;

	private AnimatedSprite2D _playerSprite;
	private CollisionShape2D _collisionShape;
	private Area2D _collisionArea;
	private Camera2D _camera;
	private Timer _shootTimer;
	private ProgressBar _healthBar;
	private Vector2 _facing = Vector2.Down;
	private bool _canShoot = true;
	private bool _isShooting = false;
	private Timer _dashTimer;
	private Timer _dashCooldownTimer;

	public override void _Ready() 
	{
		GD.Randomize();
		_health = MaxHealth;
		_playerSprite = GetNode<AnimatedSprite2D>("PlayerSprite");
		_collisionShape = GetNode<CollisionShape2D>("Shape");
		_camera = GetNode<Camera2D>("Camera");
		_shootTimer = GetNode<Timer>("ShootTimer");
		_collisionArea = GetNode<Area2D>("Area");
		_healthBar = GetNode<ProgressBar>("HealthBar");
		
		_healthBar.Visible = false;
		_shootTimer.WaitTime = FireRate;
		

		_shootTimer.Timeout += _OnShootTimerTimeout;
		_collisionArea.BodyEntered += _OnAreaBodyEntered;
		_dashTimer = GetNode<Timer>("DashTimer");
		_dashTimer.WaitTime = DashDuration;
		_dashTimer.Timeout += ResetDash;
		_dashTimer.OneShot = true;
		_dashCooldownTimer = GetNode<Timer>("DashCooldownTimer");
		_dashCooldownTimer.WaitTime = DashCooldown;
		_dashCooldownTimer.OneShot = true;
		_dashCooldownTimer.Timeout += ResetDashCooldown;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("shoot"))
		{
			if (_canShoot)
			{
				Shoot();
				_shootTimer.Start();
			}
			_isShooting = true;
		}
		else if (@event.IsActionReleased("shoot"))
		{
			_isShooting = false;
		}

		if (@event.IsActionPressed("dash"))
		{
			if (_canDash) Dash();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		GetInput();
		MoveAndSlide();
	}

	private void GetInput()
	{
		Vector2 inputDirection = Input.GetVector("walk_left", "walk_right", "walk_up", "walk_down");
		Velocity = inputDirection * Speed;
		ChangeAnimation(inputDirection);
	}

	private void ChangeAnimation(Vector2 inputDirection)
	{
		if (inputDirection == Vector2.Zero)
		{
			StopWalking();
		}
		else
		{
			_facing = inputDirection;

			string animation = inputDirection switch
			{
				var dir when dir.X > 0 => "walk_right",
				var dir when dir.X < 0 => "walk_left",
				var dir when dir.Y > 0 => "walk_front",
				var dir when dir.Y < 0 => "walk_back",
				_ => _playerSprite.Animation
			};

			if (_playerSprite.Animation != animation)
			{
				_playerSprite.Animation = animation;
				_playerSprite.Play();
			}
		}
	}

	private void StopWalking()
	{
		string animation = _facing switch
		{
			var dir when dir == Vector2.Down => "stand_front",
			var dir when dir == Vector2.Left => "stand_left",
			var dir when dir == Vector2.Up => "stand_back",
			var dir when dir == Vector2.Right => "stand_right",
			_ => "stand_front"
		};

		_playerSprite.Animation = animation;
	}

	private void Shoot()
	{
		_canShoot = false;
		Vector2 direction = GetShootingDirection();
		CreateBullet(direction);
	}

	private Vector2 GetShootingDirection()
	{
		Vector2 mousePos = GetGlobalMousePosition();
		return (mousePos - GlobalPosition).Normalized();
	}

	private void CreateBullet(Vector2 direction)
	{
		PackedScene bulletScene = GD.Load<PackedScene>("res://scenes/bullet.tscn");
		if (bulletScene == null) return;

		Shootemmono.scripts.Entities.Bullet bullet = bulletScene.Instantiate<Shootemmono.scripts.Entities.Bullet>();
		GetParent().AddChild(bullet);

		bullet.GlobalPosition = GlobalPosition + (new Vector2(8, 8) * direction);
		bullet.Setup(direction, Speed, this);
	}

	private void _OnShootTimerTimeout()
	{
		_canShoot = true;
		if (_isShooting)
		{
			Shoot();
		}
		_shootTimer.Start();
	}

	private void _OnAreaBodyEntered(Node2D body)
	{
		// GD.Print("Enemy entered the player");
		if (body.GetGroups().Contains("enemy"))
		{
			if (_health <= 0) Shootemmono.scripts.Autoload.GameEvents.Instance.EmitGameOver();
			else
			{
				_health -= 10;
				if (_health <= 0)
				{
					Shootemmono.scripts.Autoload.GameEvents.Instance.EmitGameOver();
					return;
				}
				_healthBar.Visible = true;
				_healthBar.Value = _health / MaxHealth * 100;
				body.QueueFree();
			}
		}
	}

	public void ActivateCamera()
	{
		_camera.MakeCurrent();
	}

	public void Heal()
	{
		_health += RegenRate;
		if (_health < MaxHealth)
		{
			_healthBar.Value = _health / MaxHealth * 100;
		}
		else
		{
			_health = MaxHealth;
			_healthBar.Visible = false;
		}
	}

	private void Dash()
	{
		GD.Print("Dashed");
		Speed *= DashSpeedMultiplier;
		_canDash = false;
		_dashTimer.Start();
	}

	private void ResetDash()
	{
		GD.Print("Dash Reset");
		Speed /= DashSpeedMultiplier;
		_dashCooldownTimer.Start();
	}

	private void ResetDashCooldown()
	{
		GD.Print("Cooldown Reset");
		_canDash = true;
	}
}

using Godot;

namespace Shootemmono.scripts.Entities;
public partial class Bullet : CharacterBody2D
{
    [Export]
    public float DespawnInNSeconds = 1.5f;

    [Export]
    public float Speed = 250.0f;

    [Export] public float Damage = 25.0f;

    private Player _shooter;
    private Sprite2D _sprite;
    private CollisionShape2D _shape;
    private Timer _timer;

    private Vector2 _direction;

    public override void _Ready()
    {
        _timer = GetNode<Timer>("Despawn");
        _timer.WaitTime = DespawnInNSeconds;
        _timer.Timeout += OnDespawnTimeout;
        
        _sprite = GetNode<Sprite2D>("Sprite");

        _shape = GetNode<CollisionShape2D>("Shape");
    }

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();
    }

    private Bullet SetDirection(Vector2 direction)
    {
        _direction = direction;
        return this;
    }

    private Bullet SetSprite()
    {
        switch (_direction)
        {
            case (-1, 0):
                _sprite.FlipH = true;
                break;
            case (1, 0):
                break;
            case (0, -1):
                _sprite.Rotation = Mathf.DegToRad(-90);
                break;
            case (0, 1):
                _sprite.Rotation = Mathf.DegToRad(90);
                break;
        }
        return this;
    }

    private Bullet StartTimer()
    {
        _timer.Start();
        return this;
    } 

    public void Setup(Vector2 direction, float sourceSpeed, CharacterBody2D shooter)
    {
        SetDirection(direction)
            .SetShooter(shooter)
            .SetSprite()
            .StartTimer();
        Velocity = direction * (Speed + sourceSpeed);
    }

    private void OnDespawnTimeout()
    {
        QueueFree();
    }

    private Bullet SetShooter(CharacterBody2D player)
    {
        _shooter = player as Player;
        return this;
    }

    public Player GetShooter()
    {
        return _shooter;
    }
}

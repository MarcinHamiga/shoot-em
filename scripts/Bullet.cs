using Godot;
using System;

namespace shootem;
public partial class Bullet : CharacterBody2D
{
    [Export]
    public float DespawnInNSeconds = 3;

    [Export]
    public float Speed = 250.0f;

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
            case Vector2(-1, 0):
                _sprite.FlipH = true;
                break;
            case Vector2(1, 0):
                break;
            case Vector2(0, -1):
                _sprite.Rotation = Mathf.DegToRad(-90);
                break;
            case Vector2(0, 1):
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

    public void Setup(Vector2 direction, float sourceSpeed)
    {
        this.SetDirection(direction)
            .SetSprite()
            .StartTimer();
        Velocity = direction * (Speed + sourceSpeed);
    }

    private void OnDespawnTimeout()
    {
        QueueFree();
    }
}

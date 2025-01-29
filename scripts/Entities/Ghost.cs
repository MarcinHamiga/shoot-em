using Godot;
using Shootemmono.scripts.Abstracts;
using Shootemmono.scripts.Autoload;

namespace Shootemmono.scripts.Entities;
public partial class Ghost : Enemy
{
    [Export]
    public float MaxHealth = 100.0f;

    private const float Speed = 95.0f;
    
    private float _health;
    private Player _target;
    private Area2D _area;
    private ProgressBar _healthBar;

    public override void _Ready()
    {
        _area = GetNode<Area2D>("Area");
        _area.BodyEntered += OnHit;
        _health = MaxHealth;
        
        _healthBar = GetNode<ProgressBar>("HealthBar");
        _healthBar.MaxValue = MaxHealth;
        _healthBar.Value = _health;
        _healthBar.Visible = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        var direction = (_target.Position - Position).Normalized();
        Velocity = direction * Speed;
        MoveAndSlide();
    }

    public void Setup(Player player)
    {
        _target = player;
    }

    protected override void OnHit(Node2D body)
    {
        Godot.Collections.Array<StringName> bodyGroups = body.GetGroups();
        if (!bodyGroups.Contains("bullet")) return;
        var bullet = body as Bullet;
        if (bullet == null) return;
        
        bullet.SetPhysicsProcess(false);
        bullet.GetShooter()
            .Heal();
        
        _health -= bullet.Damage;
        _healthBar.Value = _health;
        _healthBar.Visible = true;
        if (_health <= 0)
        {
            SetPhysicsProcess(false);
            GameEvents.Instance.EmitEnemyKilled(this, bullet);
        }
        else
        {
            bullet.QueueFree();
        }
    }
}
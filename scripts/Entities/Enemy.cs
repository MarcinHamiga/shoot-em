using Godot;
using System;

namespace shootem;
public partial class Enemy : CharacterBody2D
{
    const float SPEED = 95.0f;
    private Player _target;
    private Area2D _area;
    private GameEvents _gameEvents;

    public override void _Ready()
    {
        _gameEvents = GetNode<GameEvents>("/root/GameEvents");
        _area = GetNode<Area2D>("Area");
        _area.BodyEntered += OnAreaBodyEntered;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 direction = (_target.Position - Position).Normalized();
        Velocity = direction * SPEED;
        MoveAndSlide();
    }

    public void Setup(Player player)
    {
        _target = player;
    }

    private void OnAreaBodyEntered(Node2D body)
    {
        Godot.Collections.Array<StringName> bodyGroups = body.GetGroups();
        if (bodyGroups.Contains("bullet"))
        {
            body.SetPhysicsProcess(false);
            SetPhysicsProcess(false);
            _gameEvents.EmitSignal("EnemyKilled", this, body);
        }
    }
}
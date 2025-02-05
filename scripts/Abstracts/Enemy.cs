using Godot;

namespace Shootemmono.scripts.Abstracts;

public abstract partial class Enemy : CharacterBody2D
{
    [Export]
    public float MaxHealth;
    
    private float _health;

    protected abstract void OnHit(Node2D body);
}
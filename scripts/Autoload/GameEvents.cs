using Godot;
using Shootemmono.scripts.Entities;

namespace Shootemmono.scripts.Autoload;

[GlobalClass]
public partial class GameEvents : Node
{
    public static GameEvents Instance { get; private set; }
    [Signal]
    public delegate void GameOverEventHandler();

    public void EmitGameOver()
    {
        EmitSignal(SignalName.GameOver);
    }

    [Signal]
    public delegate void EnemyKilledEventHandler(Ghost ghost, Bullet bullet);

    public void EmitEnemyKilled(Ghost ghost, Bullet bullet)
    {
        EmitSignal(SignalName.EnemyKilled, ghost, bullet);
    }

    public override void _EnterTree()
    {
        Instance = this;
        base._EnterTree();
    }
}
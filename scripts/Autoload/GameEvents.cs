using Godot;

namespace shootem;

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
    public delegate void EnemyKilledEventHandler(Enemy enemy, Bullet bullet);

    public void EmitEnemyKilled(Enemy enemy, Bullet bullet)
    {
        EmitSignal(SignalName.EnemyKilled, enemy, bullet);
    }

    public override void _EnterTree()
    {
        Instance = this;
        base._EnterTree();
    }
}
using Godot;

namespace shootem;
public partial class GameEvents : Node
{
    [Signal]
    public delegate void GameOverEventHandler();

    [Signal]
    public delegate void EnemyKilledEventHandler(Enemy enemy, Bullet bullet);
}
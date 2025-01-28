using Godot;
using Godot.Collections;

namespace shootem;
public partial class Game : Node
{
    [Export]
    public float SpawnInterval { get; set; } = 2.0f;
    private Timer _spawnTimer;
    private Node _entities;
    private Player _player;
    private Label _scoreLabel;
    private Node _spawnPoints;

    private int _score = 0;
    private bool _startingNewGame = false;
    private GameEvents _gameEvents;
    public override void _Ready()
    {
        GD.Randomize();

        _spawnTimer = GetNode<Timer>("SpawnTimer");
        _entities = GetNode<Node>("Entities");
        _player = GetNode<Player>("Entities/PlayerBody");
        _scoreLabel = GetNode<Label>("UI/UIMargin/Score");
        _spawnPoints = GetNode<Node>("SpawnPoints");

        _player.ActivateCamera();
        _spawnTimer.WaitTime = SpawnInterval;
        _spawnTimer.Timeout += _OnSpawnTimerTimeout;
        _spawnTimer.Start();

        var gameEvents = GetNode<GameEvents>("/root/GameEvents");
        gameEvents.EnemyKilled += _OnEnemyKilled;
        gameEvents.GameOver += _OnGameOver;
    }

    private void _OnSpawnTimerTimeout()
    {
        Array<Node> spawners = _spawnPoints.GetChildren();
        int randomIndex = GD.RandRange(0, spawners.Count - 1);

        if (spawners[randomIndex] is Spawner spawner)
        {
            spawner.Spawn(_entities, _player);
            _spawnTimer.Start();
        }
    }

    private void NewGame()
    {
        foreach (Node child in _entities.GetChildren())
        {
            child.QueueFree();
        }

        CallDeferred(nameof(_AddPlayer));
        _score = 0;
        _scoreLabel.Text = $"Score: {_score}";
        _spawnTimer.Stop();
        _spawnTimer.Start();
        _startingNewGame = false;
    }

    private void _AddPlayer()
    {
        PackedScene playerScene = GD.Load<PackedScene>("res://scenes/player.tscn");
        _player = playerScene.Instantiate<Player>();
        _entities.AddChild(_player);
        _player.Position = new Vector2(256, 256);
    }

    private void _OnGameOver()
    {
        if (!_startingNewGame)
        {
            _startingNewGame = true;
            CallDeferred(nameof(NewGame));
        }
    }

    private void _OnEnemyKilled(Enemy enemy, Bullet bullet)
    {
        _score += 10;
        _scoreLabel.Text = $"Score: {_score}";
        bullet.QueueFree();
        enemy.QueueFree();
    }

}
using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class Game : Node
{
    [Export]
    private Array<PackedScene> Levels;

    [Export]
    private Node LevelsContainer;

    [Export]
    private Player Player;

    public override void _Ready()
    {
        base._Ready();
        GD.PrintErr("got here");
        AddLevel();
    }

    private uint currentLevelNumber = 0;
    private Stack<Level> LoadedLevels = new Stack<Level>();
    private void AddLevel()
    {
        Level currentLevel = Levels[(int)(GD.Randi() % Levels.Count)].Instantiate<Level>();

        currentLevel.TriggerArea.BodyEntered += OnLevelEntered;
        currentLevel.TriggerArea.BodyExited += OnLevelExited;

        currentLevel.Position = new Vector2(0, -1024 - (512 * currentLevelNumber));

        LoadedLevels.Push(currentLevel);
        LevelsContainer.CallDeferred(MethodName.AddChild, currentLevel);

        currentLevelNumber++;
    }

    private void OnLevelEntered(Node2D body)
    {
        if (body is Player)
        {
            Level currentLevel = LoadedLevels.Peek();
            currentLevel.TriggerArea.BodyEntered -= OnLevelEntered;

            AddLevel();
        }
    }

    private void OnLevelExited(Node2D body)
    {
        if (body is Player && body.GlobalPosition.Y > -256 - (512 * currentLevelNumber))
        {
            Level toUnload = LoadedLevels.Pop();
            LevelsContainer.CallDeferred(MethodName.RemoveChild, toUnload);

            Level currentLevel = LoadedLevels.Peek();

            currentLevel.TriggerArea.BodyEntered += OnLevelEntered;
            currentLevelNumber--;
        }
    }
}

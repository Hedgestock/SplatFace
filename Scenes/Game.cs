using Godot;
using Godot.Collections;
using System;

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
        AddLevel();
    }

    private uint currentLevelNumber = 0;
    private Level currentLevel;
    private void AddLevel()
    {

        currentLevel = Levels[(int) GD.Randi() % Levels.Count].Instantiate<Level>();

        currentLevel.TriggerArea.BodyEntered += OnLevelTrigger;

        currentLevel.Position = new Vector2(0, -1024 - (512 * currentLevelNumber));

        LevelsContainer.CallDeferred(MethodName.AddChild, currentLevel);

        currentLevelNumber++;
    }

    private void OnLevelTrigger(Node2D body)
    {
        if (body is Player)
        {
            GD.Print("adding level");
            if (currentLevel != null)
            {
                currentLevel.TriggerArea.BodyEntered -= OnLevelTrigger;
            }
            AddLevel();
        }
    }
}

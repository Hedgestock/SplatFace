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
        AddLevel();
    }

    private uint currentLevelNumber = 0;
    private Stack<Level> LoadedLevels = new Stack<Level>();
    private void AddLevel()
    {
        Level currentLevel = Levels[(int)(GD.Randi() % Levels.Count)].Instantiate<Level>();

        GD.Print("AddLevel start ", currentLevelNumber," " ,  currentLevel.Name);

        currentLevel.TriggerArea.BodyEntered += OnNextLevelTrigger;

        currentLevel.Position = new Vector2(0, -1024 - (512 * currentLevelNumber));

        LoadedLevels.Push(currentLevel);
        LevelsContainer.CallDeferred(MethodName.AddChild, currentLevel);

        currentLevelNumber++;
        GD.Print("AddLevel end ", currentLevelNumber, " ", currentLevel.Name);

    }

    private void OnNextLevelTrigger(Node2D body)
    {
        if (body is Player)
        {
            GD.Print("Next Trigger");

            Level currentLevel = LoadedLevels.Peek();
            currentLevel.TriggerArea.BodyEntered -= OnNextLevelTrigger;
            currentLevel.TriggerArea.BodyEntered += OnPreviousLevelTrigger;
            AddLevel();
        }
    }


    private void OnPreviousLevelTrigger(Node2D body)
    {
        if (body is Player)
        {
            GD.Print("Previous Trigger Start ", currentLevelNumber);

            Level toUnload = LoadedLevels.Pop();
            LevelsContainer.CallDeferred(MethodName.RemoveChild, toUnload);

            Level currentLevel = LoadedLevels.Peek();

            currentLevel.TriggerArea.BodyEntered -= OnPreviousLevelTrigger;
            currentLevel.TriggerArea.BodyEntered += OnNextLevelTrigger;
            currentLevelNumber--;

            GD.Print("Previous Trigger End ", currentLevelNumber);
        }
    }
}

using Godot;
using System;

public partial class ScoreLabel : Label
{
    [Export]
    private ScoreSourceOfTruth Score;

    public override void _Ready()
    {
        base._Ready();

        Score.ScoreChanged += OnScoreChanged;
    }

    void OnScoreChanged()
    {
        Text =  $"{Score.Score:0.00} m";
    }
}

using Godot;
using System;

public partial class Game : Node
{
    [Export]
    private Label Score;

    private double _maxScore = 0;

    private void HandleScore(double score)
    {
        if (_maxScore > score) return;
        _maxScore = score;
        Score.Text = $"{score:0.00} m";
    }
}

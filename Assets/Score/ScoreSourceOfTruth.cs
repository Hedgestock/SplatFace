using Godot;
using System;

[GlobalClass]
public partial class ScoreSourceOfTruth : Resource
{
    [Signal]
    public delegate void ScoreChangedEventHandler();


    private double _score;

    [Export]
    public double Score
    {
        get { return _score; }
        set
        {
            if (_score >= value) return;
            _score = value;
            EmitSignal(SignalName.ScoreChanged);
        }
    }
}

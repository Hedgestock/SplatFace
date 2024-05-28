using Godot;
using System;

public partial class DebugWindow : Window
{
    public override void _Ready()
    {
        base._Ready();
        GetViewport().World2D = GetTree().Root.World2D;
    }
}

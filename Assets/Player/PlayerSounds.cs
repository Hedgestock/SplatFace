using Godot;
using System;

public partial class PlayerSounds : AudioStreamPlayer2D
{
    [Export]
    AudioStream Running;

    [Export]
    AudioStream Splatting;

    [Export]
    AudioStream Landing;

    private void Run()
    {
        Stream = Running;
        Play();
    }

    private void Splat()
    {
        Stream = Splatting;
        Play();
    }

    private void Land()
    {
        Stream = Landing;
        Play();
    }
}

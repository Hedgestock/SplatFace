using Godot;
using System;
using System.Collections.Generic;
using static Godot.WebSocketPeer;

public partial class Player : RigidBody2D
{
    [Export]
    int Speed = 200;

    [Export]
    private ScoreSourceOfTruth Score;

    [Export]
    Label DistanceFallen;

    [Export]
    AudioStreamPlayer Music;

    [ExportGroup("Animation")]
    [Export]
    AnimatedSprite2D Sprite;
    [Export]
    AnimationPlayer Animation;

    [ExportGroup("Physics")]
    [Export]
    RayCast2D GroundRayCast;
    [Export]
    RayCast2D RightRayCast;
    [Export]
    RayCast2D LeftRayCast;

    [Export]
    PhysicsMaterial ControlledPhysics;
    [Export]
    PhysicsMaterial RagdollPhysics;

    [ExportGroup("Debug")]
    [Export]
    Label DebugLabel;
    [Export]
    Window DebugWindow;

    private float FallingHeight = 0;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        GroundRayCast.TargetPosition = new Vector2(0, (float)Mathf.Max(LinearVelocity.Y * 2 * delta, 5));

        if (IsActionable)
        {
            float direction = Input.GetAxis("move_left", "move_right");

            if (IsWalking)
                direction = (float)Mathf.Clamp(direction, -0.49, 0.49);

            LinearVelocity = new Vector2((float)(direction * Speed), LinearVelocity.Y);
            if (Animation.AssignedAnimation != "jump")
            {
                if (direction == 0) Animation.Play("idle");
                else
                {
                    if (Mathf.Abs(direction) < 0.5 && (Animation.AssignedAnimation != "run")) Animation.Play("walk");
                    else if (Animation.AssignedAnimation != "run") Animation.Play("run");

                    Sprite.FlipH = direction < 0;
                }
            }
        }
        else
        {
            if (CanFall && LinearVelocity.Y >= 0)
            {
                Fall();
            }
            else if (Animation.AssignedAnimation == "fall" && GroundRayCast.IsColliding())
            {
                Land();
            } 
            else if (Animation.AssignedAnimation == "sit" && Input.GetActionStrength("sit") != 1)
            {
                Idle();
            }
        }

        if (ShouldRagdoll) { PhysicsMaterialOverride = RagdollPhysics; }
        else { PhysicsMaterialOverride = ControlledPhysics; }

        DebugLabel.Text = $"{Animation.AssignedAnimation} - {Sprite.Animation}\ngrounded: {GroundRayCast.IsColliding()}\nactionable: {IsActionable}\n{LinearVelocity.Y:0.0}";
    }

    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        base._IntegrateForces(state);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("jump"))
        {
            JumpSquat();
        }
        if (@event.IsAction("sit") && IsActionable) {
            Sit();
        }
        if (@event.IsActionPressed("pause"))
        {
            DebugWindow.Visible = !DebugWindow.Visible;
        }
    }

    void JumpSquat()
    {
        if (IsActionable)
        {
            if (IsWalking) _lockWalkState = true;
            Animation.Play("jump");
        }
    }

    void Jump()
    {
        if (!GroundRayCast.IsColliding()) return;

        _lockWalkState = false;
        Sprite.Play("jump");
        DistanceFallen.Hide();
        LinearVelocity = new Vector2(LinearVelocity.X, -500);
    }

    void Fall()
    {
        FallingHeight = Position.Y;
        Animation.Play("fall");
        DistanceFallen.Hide();
    }

    void Land()
    {
        double distanceFallen = Math.Abs(FallingHeight - Position.Y) / 15;
        if (distanceFallen > 10)
        {
            DistanceFallen.Show();
            DistanceFallen.Text = $"{distanceFallen:0.00} m";

            Score.Score = distanceFallen;

            Animation.Play("splat");
        }
        else
        {
            Animation.Play("land");
        }
    }

    void Sit()
    {
        Animation.Play("sit");
    }

    void Idle()
    {
        Animation.Play("idle");
    }

    void ToggleMusic()
    {
        Music.StreamPaused = !Music.StreamPaused;
    }

    bool IsActionable
    { get { return GroundRayCast.IsColliding() && !new List<string>() { "jump", "fall", "land", "splat", "sit" }.Contains(Sprite.Animation); } }

    bool CanFall
    { get { return !new List<string>() { "fall", "land", "splat", "sit" }.Contains(Sprite.Animation); } }

    private bool _lockWalkState = false;

    bool IsWalking
    { get { return Input.IsActionPressed("walk") || Sprite.Animation == "walk" || _lockWalkState; } }

    bool ShouldRagdoll
    { get { return !GroundRayCast.IsColliding() || Sprite.Animation == "jump"; } }
}

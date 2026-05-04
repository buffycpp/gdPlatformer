using System;
using Godot;

public partial class Player : CharacterBody2D
{
    [Export] public float Speed = 200f;
    [Export] public float JumpVelocity = -400f;
    [Export] public float Gravity = 900f;

    [Export] public float AirControl = 0.08f;

    [Export] public float WallSlideSpeed = 100f;
    [Export] public float WallJumpForceX = 320f;
    [Export] public float WallJumpForceY = -420f;
    [Export] public float WallJumpLockTime = 0.25f;

    [Export] public float SnapLength = 12f;
    [Export] public float FloorMaxAngleCfg = 60f;

    // NEW: ground smoothing strength (very small = tight feel)
    [Export] public float GroundSmoothing = 18f;
    [Export] public AnimatedSprite2D animatedSprite;
    [Export] public CollisionShape2D collisionShape2D;
    [Export] public float AliveTime = 0;

    private float _wallJumpLockTimer = 0f;
    private bool _isWallSliding = false;
    private float _lastDeathTime = 0;
    private float _deathTimer = 0;
    public bool IsDead { get; private set; } = false;
    public bool IsDormant { get; private set; } = false;

    //Input
    public float _xInput = 0;
    public bool _jumpQueued = false;

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;
        float dt = (float)delta;

        if (IsDormant || IsDead)
        {
            return;
        }

        AliveTime += dt;

        // --------------------
        // GRAVITY
        // --------------------
        if (!IsOnFloor())
            velocity.Y += Gravity * dt;

        // --------------------
        // WALL JUMP LOCK TIMER
        // --------------------
        if (_wallJumpLockTimer > 0)
            _wallJumpLockTimer -= dt;

        // --------------------
        // INPUT
        // --------------------
        bool onFloor = IsOnFloor();
        bool onWall = IsOnWall() && !onFloor;

        // --------------------
        // WALL SLIDE
        // --------------------
        if (onWall && _xInput != 0)
        {
            _isWallSliding = true;
            velocity.Y = Mathf.Min(velocity.Y, WallSlideSpeed);
        }
        else
        {
            _isWallSliding = false;
        }

        // --------------------
        // HORIZONTAL MOVEMENT
        // --------------------
        float targetX = _xInput * Speed;

        if (_wallJumpLockTimer > 0)
        {
            // no control during lock
        }
        else if (onFloor)
        {
            // VERY SLIGHT smoothing instead of instant snap
            velocity.X = Mathf.Lerp(velocity.X, targetX, GroundSmoothing * dt);
        }
        else
        {
            // air control stays as-is (important for your feel)
            velocity.X = Mathf.Lerp(velocity.X, targetX, AirControl);
        }

        // --------------------
        // JUMPING
        // --------------------
        if (_jumpQueued)
        {
            if (onFloor)
            {
                velocity.Y = JumpVelocity;
            }
            else if (_isWallSliding)
            {
                float wallDir = GetWallNormal().X;

                velocity.X = wallDir * WallJumpForceX;
                velocity.Y = WallJumpForceY;

                _wallJumpLockTimer = WallJumpLockTime;
                _isWallSliding = false;
            }

            _jumpQueued = false;
        }

        // --------------------
        // SLOPE SETTINGS
        // --------------------
        FloorSnapLength = SnapLength;
        FloorMaxAngle = Mathf.DegToRad(FloorMaxAngleCfg);
        FloorStopOnSlope = true;
        SafeMargin = 0.05f;

        // --------------------
        // APPLY
        // --------------------
        Velocity = velocity;
        UpdateAnimation(velocity);

        MoveAndSlide();
    }

    public void UpdateAnimation(Vector2 velocity)
    {
        if (velocity.LengthSquared() < 5f)
        {
            animatedSprite.Play("idle");
        }
        else
        {
            if (velocity.Y != 0)
            {
                if (velocity.Y < 0)
                {
                    animatedSprite.Play("jumping");
                }
                else if (velocity.Y > 0)
                {
                    animatedSprite.Play("falling");
                }
            }
            else
            {
                if (velocity.X > 0 || velocity.X < 0)
                {
                    animatedSprite.Play("run");
                }
            }
        }

        animatedSprite.FlipH = velocity.X < 0;
        animatedSprite.Offset = animatedSprite.FlipH ? new Vector2(4, 0) : Vector2.Zero;
    }

    public void SetDormance(bool isDormant)
    {
        IsDormant = isDormant;
        collisionShape2D.Disabled = IsDormant;
    }

    public void QueueJump()
    {
        _jumpQueued = true;
    }

    public void SetXInput(float input)
    {
        _xInput = input;
    }

    public void SetDeath(bool isDead)
    {
        IsDead = isDead;
        AliveTime = 0;
        if (IsDead)
        {
            animatedSprite.Stop();
        }
        else
        {
            animatedSprite.Play();
        }
    }

    public void ResetInput()
    {
        _xInput = 0;
        _jumpQueued = false;
    }

    public void ForcePosition(Vector2 position)
    {
        Velocity = Vector2.Zero;
        UpdateAnimation(Velocity);
        GlobalPosition = position;
    }
}
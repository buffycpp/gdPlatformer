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
    public float AliveTime { get; set; } = 0;
    [Export] public AudioStream JumpSound;
    [Export] public AudioStream FootstepSound;
    [Export] public AudioStreamPlayer2D StreamPlayer { get; set; }
    [Export] public float FootstepFrequency { get; set; } = 0.25f;

    private float _wallJumpLockTimer = 0f;
    private bool _isWallSliding = false;
    private float _lastDeathTime = 0;
    private float _deathTimer = 0;
    public bool IsDead { get; private set; } = false;
    public bool IsDormant { get; private set; } = false;
    private Vector2? _moveDestination = null;
    private Action _onDestinationReached = null;
    private float _footstepTimer = 0;

    //Input
    public float _xInput = 0;
    public bool _jumpQueued = false;

    public override void _PhysicsProcess(double delta)
    {

        float fDelta = (float)delta;

        if (IsDormant || IsDead)
        {
            return;
        }

        AliveTime += fDelta;

        Vector2 velocity;
        if (_moveDestination == null)
        {
            velocity = MoveWithInput(fDelta);
        }
        else
        {
            velocity = MoveWithDestination(fDelta);
        }

        // --------------------
        // SLOPE SETTINGS
        // --------------------
        FloorSnapLength = SnapLength;
        FloorMaxAngle = Mathf.DegToRad(FloorMaxAngleCfg);
        FloorStopOnSlope = true;
        SafeMargin = 0.05f;

        Velocity = velocity;
        UpdateAnimation(velocity);

        MoveAndSlide();

        if (_moveDestination != null)
        {
            if (GlobalPosition.DistanceTo(_moveDestination.Value) < 3f)
            {
                _moveDestination = null;
                _onDestinationReached?.Invoke();
            }
        }

        HandleFootsteps(fDelta);
    }

    public Vector2 MoveWithDestination(float delta)
    {
        if (_moveDestination == null)
        {
            return Vector2.Zero;
        }

        Vector2 velocity = Velocity;
        bool onFloor = IsOnFloor();

        //gravity                
        if (!onFloor)
        {
            velocity.Y += Gravity * delta;
        }

        float direction = 0;
        if (_moveDestination.Value.X < GlobalPosition.X)
        {
            direction = -1f;
        }
        else if (_moveDestination.Value.X > GlobalPosition.X)
        {
            direction = 1;
        }

        //horizontal movement
        float targetX = direction * Speed;

        if (_wallJumpLockTimer > 0)
        {
            // no control during lock
        }
        else if (onFloor)
        {
            // VERY SLIGHT smoothing instead of instant snap
            velocity.X = Mathf.Lerp(velocity.X, targetX, GroundSmoothing * delta);
        }
        else
        {
            // air control stays as is
            velocity.X = Mathf.Lerp(velocity.X, targetX, AirControl);
        }

        return velocity;
    }

    public Vector2 MoveWithInput(float delta)
    {
        Vector2 velocity = Velocity;
        bool onFloor = IsOnFloor();

        //gravity                
        if (!onFloor)
        {
            velocity.Y += Gravity * delta;
        }

        //walljump lock timer
        if (_wallJumpLockTimer > 0)
        {
            _wallJumpLockTimer -= delta;
        }

        bool onWall = IsOnWall() && !onFloor;

        //wall slide
        if (onWall && _xInput != 0)
        {
            _isWallSliding = true;
            velocity.Y = Mathf.Min(velocity.Y, WallSlideSpeed);
        }
        else
        {
            _isWallSliding = false;
        }

        //horizontal movement
        float targetX = _xInput * Speed;

        if (_wallJumpLockTimer > 0)
        {
            // no control during lock
        }
        else if (onFloor)
        {
            // VERY SLIGHT smoothing instead of instant snap
            velocity.X = Mathf.Lerp(velocity.X, targetX, GroundSmoothing * delta);
        }
        else
        {
            // air control stays as is
            velocity.X = Mathf.Lerp(velocity.X, targetX, AirControl);
        }

        //jumping
        if (_jumpQueued)
        {
            if (onFloor)
            {
                velocity.Y = JumpVelocity;
                PlayJumpSFX();
            }
            else if (_isWallSliding)
            {
                float wallDir = GetWallNormal().X;

                velocity.X = wallDir * WallJumpForceX;
                velocity.Y = WallJumpForceY;

                _wallJumpLockTimer = WallJumpLockTime;
                _isWallSliding = false;
                PlayJumpSFX();
            }



            _jumpQueued = false;
        }

        //only play sound if sliding down, not up
        if (_isWallSliding && velocity.Y > 0)
        {
            if (!StreamPlayer.Playing)
                StreamPlayer.Play();
        }
        else
        {
            if (StreamPlayer.Playing)
                StreamPlayer.Stop();
        }

        return velocity;
    }

    public void HandleFootsteps(float delta)
    {
        _footstepTimer += delta;
        if (!IsOnFloor() || _footstepTimer < FootstepFrequency)
        {
            return;
        }

        //if (Math.Abs(Velocity.X) > 1f)
        if (_xInput != 0)
        {
            SoundManager.Instance.PlaySfx(FootstepSound, GlobalPosition, -10f, (float)GD.RandRange(0.9, 1.1));
            _footstepTimer = 0;
        }
    }

    public void PlayJumpSFX()
    {
        if (JumpSound != null)
        {
            SoundManager.Instance.PlaySfx(JumpSound, GlobalPosition, 0, (float)GD.RandRange(0.9, 1.1));
        }
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

    public void WalkTo(Vector2 position, Action onDestinationReached)
    {
        _moveDestination = position;
        _onDestinationReached = onDestinationReached;
    }
}
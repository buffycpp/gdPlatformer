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

    private float _wallJumpLockTimer = 0f;
    private bool _isWallSliding = false;
    private float _lastDeathTime = 0;
    private float _deathTimer = 0;
    public bool IsDead { get; private set; } = false;

    public override void _Ready()
    {
        GameController.Instance.PlayerDeathEvent += OnPlayerDeath;
        GameController.Instance.PlayerSpawnEvent += OnPlayerSpawn;
    }

    public override void _PhysicsProcess(double delta)
    {

        Vector2 velocity = Velocity;
        float dt = (float)delta;

        if (IsDeathLocked(dt))
        {
            return;
        }

        if (IsDead)
        {              
            if(Input.IsActionJustPressed("jump"))
            {
                GameController.Instance.SpawnPlayer();                
            }
            
            return;
        }

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
        float input = Input.GetAxis("move_left", "move_right");

        bool onFloor = IsOnFloor();
        bool onWall = IsOnWall() && !onFloor;

        // --------------------
        // WALL SLIDE
        // --------------------
        if (onWall && input != 0)
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
        float targetX = input * Speed;

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
        if (Input.IsActionJustPressed("jump"))
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

        MoveAndSlide();
    }

    private bool IsDeathLocked(float delta)
    {
        _deathTimer += delta;
        return _lastDeathTime > 0 && _deathTimer < 1f;
    }

    private void OnPlayerDeath()
    {
        IsDead = true;
        _deathTimer = 0;
        _lastDeathTime = GameController.Instance.LevelTime;
        GameController.Instance.TimeScale = 0;
        Velocity = Vector2.Zero;
    }

    private void OnPlayerSpawn()
    {
        IsDead = false;
        GlobalPosition = Vector2.Zero;
        GameController.Instance.TimeScale = 1;
    }

    public override void _ExitTree()
    {
        GameController.Instance.PlayerDeathEvent -= OnPlayerDeath;
        GameController.Instance.PlayerDeathEvent -= OnPlayerSpawn;
    }    
}
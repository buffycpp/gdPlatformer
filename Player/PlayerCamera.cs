using Godot;

public partial class PlayerCamera : Camera2D
{
    [Export] public NodePath TargetPath;

    [Export] public float FollowSpeed = 8f;

    // Look-ahead settings
    [Export] public float LookAheadAmount = 60f;
    [Export] public float LookAheadSpeed = 6f;

    // Vertical smoothing (important for jumps)
    [Export] public float VerticalFollowSpeed = 5f;

    // Dead zone (prevents micro jitter)
    [Export] public float DeadZone = 2f;

    private Node2D _target;
    private CharacterBody2D _player;

    private Vector2 _lookAheadOffset = Vector2.Zero;
    public bool IsPaused {get; set;} = false;
    public override void _Ready()
    {
        _target = GetNode<Node2D>(TargetPath);
        _player = _target as CharacterBody2D;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_target == null || IsPaused) return;

        float dt = (float)delta;

        Vector2 targetPos = _target.GlobalPosition;

        // ----------------------------
        // LOOK AHEAD (based on velocity)
        // ----------------------------
        Vector2 velocity = Vector2.Zero;

        if (_player != null)
            velocity = _player.Velocity;

        Vector2 desiredLookAhead = new Vector2(
            Mathf.Clamp(velocity.X * 0.2f, -LookAheadAmount, LookAheadAmount),
            0
        );

        _lookAheadOffset = _lookAheadOffset.Lerp(
            desiredLookAhead,
            LookAheadSpeed * dt
        );

        // ----------------------------
        // APPLY LOOK AHEAD
        // ----------------------------
        Vector2 desiredPos = targetPos + _lookAheadOffset;

        // ----------------------------
        // DEAD ZONE (prevents micro shake)
        // ----------------------------
        Vector2 diff = desiredPos - GlobalPosition;

        if (diff.Length() < DeadZone)
            return;

        // ----------------------------
        // SMOOTH FOLLOW (X + Y separated feel)
        // ----------------------------
        Vector2 newPos = GlobalPosition;

        // Horizontal follow (snappier for platformers)
        newPos.X = Mathf.Lerp(GlobalPosition.X, desiredPos.X, FollowSpeed * dt);

        // Vertical follow (slower for jump readability)
        newPos.Y = Mathf.Lerp(GlobalPosition.Y, desiredPos.Y, VerticalFollowSpeed * dt);

        GlobalPosition = newPos;
    }

    public void ForcePosition(Vector2 position)
    {
        GlobalPosition = position;
    }
}
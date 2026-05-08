using Godot;
using System;

public partial class LevelCompleteTrigger : Area2D
{
	[Export] public AnimatedSprite2D animatedSprite;
	[Export] public PathFollow2D pathFollow2d;
	[Export] public float TransitionTime = 2f;
	[Export] public AudioStream CloseDoorSound;
	[Export] public AudioStream TubeAnimationSound;
	private CollisionShape2D _collisionShape;
	private float _animationProgress = 0;
	private bool animatingPlayer = false;

	public override void _Ready()
	{
		GameController.Instance.StartLevelEvent += OnStartLevel;
		_collisionShape = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");

		if (_collisionShape == null)
		{
			GD.PushError("DamageSource: Missing CollisionShape2D");
			return;
		}

		_collisionShape.Disabled = true;

		BodyEntered += OnBodyEntered;
	}

    public override void _Process(double delta)
    {
        if (!animatingPlayer)
		{
			return;
		}

		_animationProgress += (float)delta;
		float pathProgress = _animationProgress / TransitionTime;
		pathFollow2d.Visible = true;
		pathFollow2d.ProgressRatio = pathProgress;

		if (pathProgress >= 1f)
		{
			animatingPlayer = false;
		}
    }

	private void OnStartLevel()
	{
		_collisionShape.Disabled = false;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body.IsInGroup("Player"))
		{
			PlayerController.Instance.WalkTo(GlobalPosition, () =>
			{
				GameController.Instance.SignalLevelComplete();
				animatedSprite.ZIndex = 100;
				animatedSprite.Play();		
				SoundManager.Instance.PlaySfx(CloseDoorSound, GlobalPosition);

				GetTree().CreateTimer(0.25f).Timeout += () =>
				{
					SoundManager.Instance.PlaySfx(TubeAnimationSound, GlobalPosition);					
				};

				GetTree().CreateTimer(0.5f).Timeout += () =>
				{
					animatingPlayer = true;		
				};								
			});
		}
	}

	public override void _ExitTree()
	{
		GameController.Instance.StartLevelEvent -= OnStartLevel;
	}

}

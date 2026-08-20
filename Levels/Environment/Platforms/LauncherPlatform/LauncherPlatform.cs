using Godot;
using System;

public partial class LauncherPlatform : Area2D
{
	[Export] public AnimatedSprite2D animatedSprite2D;
	[Export] public AnimatableBody2D physicalBody;

	[Export] public float LaunchForce = 500.0f;
	[Export] public float LaunchCooldown = 0.0f;
	[Export] public float MaxExtension = 48.0f;

	private float _cooldownTimer = 0.0f;
	private bool _isLaunching = false;
	private bool _playerInLauncher = false;

	private Vector2 _originalBodyPosition;
	private float _extensionProgress = 0.0f;
	private float _extensionProgressSpeed;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;

		animatedSprite2D.AnimationFinished += OnAnimationFinished;

		_originalBodyPosition = physicalBody.Position;

		// Frame 0 -> frame 3 = 3 frame intervals.
		float animationFps =
			(float)animatedSprite2D.SpriteFrames.GetAnimationSpeed("extend");

		float animationDuration = 3.0f / animationFps;

		_extensionProgressSpeed = 1.0f / animationDuration;
	}

	public override void _PhysicsProcess(double delta)
	{
		float deltaFloat = (float)delta;

		if (_cooldownTimer > 0)
		{
			_cooldownTimer -= deltaFloat;
		}

		float targetProgress = 0.0f;

		if (animatedSprite2D.Animation == "extend")
		{
			targetProgress = 1.0f;
		}
		else if (animatedSprite2D.Animation == "retract")
		{
			targetProgress = 0.0f;
		}

		_extensionProgress = Mathf.MoveToward(
			_extensionProgress,
			targetProgress,
			_extensionProgressSpeed * deltaFloat
		);

		physicalBody.Position =
			_originalBodyPosition +
			Vector2.Up * (MaxExtension * _extensionProgress);
	}

	private void OnBodyEntered(Node2D body)
	{
		if (!body.IsInGroup("Player"))
		{
			return;
		}

		if (_cooldownTimer > 0 || _isLaunching)
		{
			return;
		}

		PlayerController.Instance.SetIsJumpBlocked(true);

		_playerInLauncher = true;
		_isLaunching = true;

		animatedSprite2D.Play("extend");
	}

	private void OnAnimationFinished()
	{
		if (animatedSprite2D.Animation != "extend")
		{
			return;
		}

		if (!_playerInLauncher)
		{
			_isLaunching = false;
			PlayerController.Instance.SetIsJumpBlocked(false);
			animatedSprite2D.Play("retract");
			return;
		}

		Vector2 launchDirection = -GlobalTransform.Y;

		PlayerController.Instance.LaunchPlayer(
			launchDirection,
			LaunchForce
		);

		_cooldownTimer = LaunchCooldown;
		_isLaunching = false;

		PlayerController.Instance.SetIsJumpBlocked(false);

		GD.Print(
			$"Launched player with force {LaunchForce} " +
			$"in direction {launchDirection}"
		);

		animatedSprite2D.Play("retract");
	}

	private void OnBodyExited(Node2D body)
	{
		if (!body.IsInGroup("Player"))
		{
			return;
		}
		_playerInLauncher = false;
		GD.Print("Player exited launcher");

		PlayerController.Instance.SetIsJumpBlocked(false);
		
		if (_isLaunching)
		{
			_isLaunching = false;
			animatedSprite2D.Play("retract");				
		}
	}
}

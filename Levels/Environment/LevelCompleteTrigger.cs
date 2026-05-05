using Godot;
using System;

public partial class LevelCompleteTrigger : Area2D
{
	[Export] public AnimatedSprite2D animatedSprite;
	private CollisionShape2D _collisionShape;

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
				GetTree().CreateTimer(0.5f).Timeout += () =>
				{
					GameController.Instance.SignalLevelComplete();
					animatedSprite.ZIndex = 100;
					animatedSprite.Play();									
				};
			});
		}
	}

	public override void _ExitTree()
	{
		GameController.Instance.StartLevelEvent -= OnStartLevel;
	}

}

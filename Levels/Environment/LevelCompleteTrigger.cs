using Godot;
using System;

public partial class LevelCompleteTrigger : Area2D
{
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
			GameController.Instance.SignalLevelComplete();
		}
	}

	public override void _ExitTree()
	{
		GameController.Instance.StartLevelEvent -= OnStartLevel;
	}

}

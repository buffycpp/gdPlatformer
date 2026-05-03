using Godot;
using System;

public partial class DamageSource : Area2D
{
	private CollisionShape2D _collisionShape;

	public override void _Ready()
	{
		_collisionShape = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");

		if (_collisionShape == null)
		{
			GD.PushError("DamageSource: Missing CollisionShape2D");
			return;
		}

		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body.IsInGroup("Player"))
		{
			GameController.Instance.HandleDamage(this);
		}
	}

	public void Toggle(bool state)
	{
		if (_collisionShape != null)
			_collisionShape.Disabled = !state;
	}
}
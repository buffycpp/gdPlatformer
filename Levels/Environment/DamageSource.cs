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
		AreaEntered += OnAreaEntered;
	}

	protected virtual void OnBodyEntered(Node2D body)
	{
		GD.Print("TurretProjectile hit: " + body.Name);
		if (body.IsInGroup("Player"))
		{
			GameController.Instance.SignalPlayerDeath(this);
		}
	}

	protected virtual void OnAreaEntered(Area2D area)
	{
		GD.Print("TurretProjectile hit: " + area.Name);
		if (area.IsInGroup("Player"))
		{
			GameController.Instance.SignalPlayerDeath(this);
		}
	}	

	public void Toggle(bool state)
	{
		if (_collisionShape != null)
			_collisionShape.Disabled = !state;
	}
}
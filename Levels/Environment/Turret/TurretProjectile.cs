using System;
using Godot;

public partial class TurretProjectile : DamageSource
{
	[Export] public float Speed = 600f;

	private Vector2 _direction;

	public override void _Ready()
	{
		base._Ready();
		_direction = Vector2.Down.Rotated(GlobalRotation);
		GameController.Instance.ResetLevelEvent += OnResetLevel;
	}

	private void OnResetLevel()
	{
		QueueFree();
	}

	public override void _ExitTree()
	{
		GameController.Instance.ResetLevelEvent -= OnResetLevel;
	}

	public override void _PhysicsProcess(double delta)
	{
		GlobalPosition += _direction * Speed * (float)delta;
	}

	protected override void OnBodyEntered(Node2D body)
	{
		base.OnBodyEntered(body);
		QueueFree();
	}
}

using Godot;
using System;
using System.Collections.ObjectModel;

public partial class CollectibleStar : Area2D
{
	[Export] public float FloatHeight = 10f;
	[Export] public float FloatSpeed = 2f;
	[Export] public PackedScene burstParticlesScene;
	[Export] public AudioStream collectionSound;

	private Vector2 _startPosition;
	private float _time;
	private CollisionShape2D _collisionShape;

	public override void _Ready()
	{
		_startPosition = Position;
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
			GameController.Instance.SignalStarCollected(this);
			Collect();
		}
	}	

	public void Collect()
	{
		var particles = (GpuParticles2D)burstParticlesScene.Instantiate();
		particles.GlobalPosition = GlobalPosition;
		particles.Finished += () =>
		{
			particles.QueueFree();
		};

		particles.Emitting = true;
		GetTree().Root.AddChild(particles);
		SoundManager.Instance.PlaySfx(collectionSound, GlobalPosition);

		QueueFree();
	}

	public override void _Process(double delta)
	{
		_time += (float)delta;

		float offset = Mathf.Sin(_time * FloatSpeed) * FloatHeight;
		Position = _startPosition + new Vector2(0, offset);
	}
}
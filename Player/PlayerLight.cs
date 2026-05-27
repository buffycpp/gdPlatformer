using Godot;
using System;

public partial class PlayerLight : LightEffect
{
	[Export] public float MaxEnergy = 0.5f;
	[Export] public float MinEnergy = 0f;
	[Export] public float DimDistance = 16f;

	private float targetEnergy;

	public override void _Ready()
	{
		base._Ready();

		targetEnergy = MaxEnergy;

		AddToGroup("Light");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		float closestDistance = float.MaxValue;

		// Find nearby lights
		foreach (Node node in GetTree().GetNodesInGroup("Light"))
		{
			if (node == this)
				continue;

			if (node is Node2D other)
			{
				float dist = GlobalPosition.DistanceTo(other.GlobalPosition);

				if (dist < closestDistance)
					closestDistance = dist;
			}
		}

		// Dim when close
		if (closestDistance < DimDistance)
		{
			float t = closestDistance / DimDistance;

			// Optional smoother falloff
			t = Mathf.Pow(t, 2.0f);

			targetEnergy = Mathf.Lerp(
				MinEnergy,
				MaxEnergy,
				t
			);
		}
		else
		{
			targetEnergy = MaxEnergy;
		}

		targetEnergy = Mathf.Clamp(
			targetEnergy,
			MinEnergy,
			MaxEnergy
		);

		// Smooth transition
		TargetEnergy = Mathf.Lerp(
			TargetEnergy,
			targetEnergy,
			(float)delta * 10f
		);
	}
}

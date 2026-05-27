using Godot;
using System;

public partial class LightEffect : PointLight2D
{
	[Export] public float Speed = 3f;
	[Export] public float EnergyVariation = 0.015f;
	[Export] public float SizeVariation = 0.015f;

	// Absolute brightness value
	public float TargetEnergy = 1f;

	private float _time;
	private float _baseScale;
	private float _offset;

	public override void _Ready()
	{
		_baseScale = TextureScale;

		// Start from current editor value
		TargetEnergy = Energy;

		// Unique offset so lights don't sync
		_offset = GD.Randf() * 1000f;
	}

	public override void _Process(double delta)
	{
		_time += (float)delta;

		float wave = Mathf.Sin(_time * Speed + _offset);

		// Flickering energy
		Energy = TargetEnergy + wave * EnergyVariation;

		// Flickering size
		TextureScale = _baseScale + wave * SizeVariation;
	}
}
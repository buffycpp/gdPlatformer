using Godot;
using System;

public partial class EffectManager : Node
{
	[Export] public BlackscreenEffect BlackscreenEffect { get; set; }
	[Export] public GrayscaleEffect GrayscaleEffect { get; set; }

	public static EffectManager Instance { get; private set; }

	public override void _EnterTree()
	{
		Instance = this;
	}


	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}

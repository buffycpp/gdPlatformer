using Godot;
using System;

public partial class EffectManager : SingletonNode<EffectManager>
{
	[Export] public BlackscreenEffect BlackscreenEffect { get; set; }
	[Export] public GrayscaleEffect GrayscaleEffect { get; set; }

	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void ExecuteWithBlackscreen(
		float fadeTime,
		Action duringBlackscreen,
		Action afterBlackscreen = null,
		float fakeDelay = 0
	)
	{
		void Leave()
		{
			BlackscreenEffect.Disable(fadeTime);

			if (afterBlackscreen != null)
			{
				GetTree().CreateTimer(fadeTime).Timeout += afterBlackscreen;
			}
		}

		BlackscreenEffect.Enable(fadeTime);
		GetTree().CreateTimer(fadeTime).Timeout += () =>
		{
			duringBlackscreen?.Invoke();
			if (fakeDelay > 0)
			{
				GetTree().CreateTimer(fakeDelay).Timeout += Leave;
			}
			else
			{
				Leave();
			}
		};
	}	
}

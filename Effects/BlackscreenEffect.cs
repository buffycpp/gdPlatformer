using Godot;
using System;

public partial class BlackscreenEffect : ColorRect
{
	private Tween currentTween;

	public void Enable(float duration = 0.25f)
	{
		Visible = true;

		currentTween?.Kill();

		Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 0f);

		currentTween = CreateTween();
		currentTween.TweenProperty(this, "modulate:a", 1f, duration)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.Out);
	}

	public void Disable(float duration = 0.25f)
	{
		currentTween?.Kill();

		currentTween = CreateTween();
		currentTween.TweenProperty(this, "modulate:a", 0f, duration)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.In);

		currentTween.Finished += () =>
		{
			Visible = false;
		};
	}
}

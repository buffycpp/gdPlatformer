using Godot;
using System;

public partial class Spikes : TimeSyncedNode2D
{
	[Export] public float AnimationSpeed = 10f;
	[Export] public float InactiveTime = 3f;
	[Export] public float ActiveTime = 3f;
	[Export] public float RetractTime = 1f;
	[Export] public bool Permanent = false;

	// 🔑 Phase offset (in seconds)
	[Export] public float PhaseOffset = 0f;

	[Export] public AnimatedSprite2D animatedSprite;
	[Export] public DamageSource damageSource;

	private float _cycleTime;
	private bool _isDangerous = false;

	public override void _Ready()
	{
		base._Ready();

		Visible = false;

		_cycleTime = InactiveTime + ActiveTime + RetractTime;

		if (Permanent)
		{
			ApplyState(visible: true, dangerous: true);
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (!_isAwake)
			return;

		if (Permanent)
		{
			return;
		}			

		float levelTime = GameController.Instance.LevelTime;

		// 🔑 Phase shift applied here (cycle space)
		float t = (levelTime + PhaseOffset) % _cycleTime;

		UpdateState(t);
	}

	private void UpdateState(float t)
	{		
		bool visible = false;
		bool dangerous = false;

		// 1. Inactive
		if (t < InactiveTime)
		{
			visible = false;
			dangerous = false;
		}
		else
		{
			t -= InactiveTime;

			// 2. Active (spikes out)
			if (t < ActiveTime)
			{
				visible = true;
				dangerous = true;

				if (!Visible)
				{
					Visible = true;
					animatedSprite.SpeedScale = AnimationSpeed;
					animatedSprite.Frame = 0;
					animatedSprite.Play();
				}
			}
			else
			{
				t -= ActiveTime;

				// 3. Retracting
				if (t < RetractTime)
				{
					visible = true;
					dangerous = false;

					if (animatedSprite.SpeedScale > 0)
					{
						animatedSprite.SpeedScale = -AnimationSpeed;
						animatedSprite.Play();
					}
				}
				else
				{
					visible = false;
					dangerous = false;
				}
			}
		}

		ApplyState(visible, dangerous);
	}

	private void ApplyState(bool visible, bool dangerous)
	{
		// Sync visuals
		if (Visible != visible)
			Visible = visible;

		// Sync gameplay
		if (_isDangerous != dangerous)
		{
			_isDangerous = dangerous;
			damageSource.Toggle(dangerous);
		}
	}
}
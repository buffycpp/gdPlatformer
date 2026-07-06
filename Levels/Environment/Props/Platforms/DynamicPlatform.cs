using Godot;
using System;

public partial class DynamicPlatform : Area2D
{
	[Export] public CollisionShape2D collisionShape2D;
	[Export] public Sprite2D sprite2D;

	[Export] public float DisappearTime = 1.0f;
	[Export] public float SemiTransparentDuration = 2.0f;

	[Export] public Texture2D noiseTexture;

	[Export] public AudioStreamPlayer2D audioStreamPlayer2D;
	[Export] public AudioStream breakSound;

	private bool _playerOnPlatform = false;
	private float _timeElapsed = 0.0f;

	private bool _playerFallenThrough = false;
	private float _fadeBackTimer = 0.0f;

	// Shake/sound sync
	private float _shakeTimer = 0.0f;
	private float _shakeInterval = 0.3f;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body.IsInGroup("Player"))
		{
			_playerOnPlatform = true;
			_timeElapsed = 0.0f;
			_shakeTimer = 0.0f;
		}
	}

	private void OnBodyExited(Node2D body)
	{
		if (body.IsInGroup("Player"))
		{
			if (_playerFallenThrough)
				return;

			ResetPlatform();
		}
	}

	public override void _Process(double delta)
	{
		float dt = (float)delta;

		// Handle respawn/fade back
		if (_playerFallenThrough)
		{
			_fadeBackTimer -= dt;

			if (_fadeBackTimer <= 0.0f)
			{
				ResetPlatform();

				_playerFallenThrough = false;
			}

			return;
		}

		if (!_playerOnPlatform)
			return;

		_timeElapsed += dt;

		float progress = Mathf.Min(_timeElapsed / DisappearTime, 1.0f);

		HandleShake(progress, dt);

		if (_timeElapsed >= DisappearTime)
		{
			BreakPlatform();
		}
	}

	private void HandleShake(float progress, float delta)
	{
		_shakeTimer -= delta;

		// Shake gets faster as it breaks
		_shakeInterval = Mathf.Lerp(0.35f, 0.08f, progress);

		if (_shakeTimer > 0.0f)
			return;

		_shakeTimer = _shakeInterval;

		float shakeStrength = Mathf.Lerp(0.2f, 1.5f, progress);

		if (sprite2D != null)
		{
			sprite2D.Position = new Vector2(
				(float)GD.RandRange(-shakeStrength, shakeStrength),
				(float)GD.RandRange(-shakeStrength, shakeStrength)
			);

			// Return sprite after shake
			var tween = CreateTween();
			tween.TweenProperty(sprite2D, "position", Vector2.Zero, 0.05f);
		}

		// Sound happens exactly when shake happens
		if (breakSound != null)
		{
			SoundManager.Instance.PlaySfx(
				breakSound,
				GlobalPosition,
				volumeDb: Mathf.Lerp(-20f, -8f, progress),
				pitch: Mathf.Lerp(0.7f, 0.8f, progress)
			);
		}
	}

	private void BreakPlatform()
	{
		_playerOnPlatform = false;

		// Disable collision
		if (collisionShape2D != null)
		{
			collisionShape2D.SetDeferred(
				CollisionShape2D.PropertyName.Disabled,
				true
			);
		}

		// Make semi-transparent
		if (sprite2D != null)
		{
			Color c = sprite2D.SelfModulate;
			c.A = 0.5f;
			sprite2D.SelfModulate = c;
		}

		// Final break sound
		if (breakSound != null)
		{
			SoundManager.Instance.PlaySfx(
				breakSound,
				GlobalPosition,
				volumeDb: 1f,
				pitch: 1.2f
			);
		}

		_playerFallenThrough = true;
		_fadeBackTimer = SemiTransparentDuration;
	}

	private void ResetPlatform()
	{
		_playerOnPlatform = false;
		_timeElapsed = 0.0f;
		_shakeTimer = 0.0f;

		if (sprite2D != null)
		{
			Color c = sprite2D.SelfModulate;
			c.A = 1.0f;
			sprite2D.SelfModulate = c;

			sprite2D.Position = Vector2.Zero;
		}

		if (collisionShape2D != null)
		{
			collisionShape2D.SetDeferred(
				CollisionShape2D.PropertyName.Disabled,
				false
			);
		}

		FadeOutAudio(0.3f);
	}

	private void FadeOutAudio(float duration)
	{
		if (audioStreamPlayer2D == null || !audioStreamPlayer2D.Playing)
			return;

		var tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Linear);

		tween.TweenProperty(
			audioStreamPlayer2D,
			"volume_db",
			-80.0f,
			duration * 2
		);

		tween.TweenCallback(Callable.From(() =>
		{
			audioStreamPlayer2D.Stop();
			audioStreamPlayer2D.VolumeDb = 0.0f;
		}));
	}
}

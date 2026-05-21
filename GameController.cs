using Godot;
using System;

public partial class GameController : Node, ISaveable
{
	public static GameController Instance { get; private set; }

	public event Action StartLevelEvent;
	public event Action PlayerDeathEvent;
	public event Action LevelCompleteEvent;
	public event Action ResetLevelEvent;

	public static float GameTime { get; private set; }

	public float TimeScale { get; set; } = 1f;

	// Level timing
	public float LevelStartTime { get; private set; }

	public float LevelTime => GameTime - LevelStartTime;

	public bool LevelRunning { get; private set; } = false;
	public int CurrentLevelIndex { get; private set; } = 0;

	public override void _EnterTree()
	{
		Instance = this;
	}

	public override void _Ready()
	{
		LevelManager.Instance.LoadLevelByIndex(CurrentLevelIndex);
	}

	public override void _Process(double delta)
	{
		if (!LevelRunning)
		{
			return;
		}

		float scaledDelta = (float)delta * TimeScale;
		GameTime += scaledDelta;
	}

	public void StartLevel()
	{
		if (LevelRunning)
		{
			return;
		}

		LevelStartTime = GameTime;
		LevelRunning = true;
		StartLevelEvent?.Invoke();
	}

	public void StopLevel()
	{
		LevelRunning = false;
	}

	public void SignalPlayerDeath(DamageSource source)
	{
		if (!LevelRunning)
		{
			return;
		}
		
		InputManager.Instance.SetInputContextDelayed(new DeathScreenInputContext(), 0.5f);
		StopLevel();
		PlayerDeathEvent?.Invoke();
		EffectManager.Instance.GrayscaleEffect.Enable();
	}

	public void SignalLevelComplete()
	{
		CallDeferred(nameof(ProcessLevelComplete));
	}

	public void ProcessLevelComplete()
	{
		StopLevel();
		LevelCompleteEvent?.Invoke();

		GetTree().CreateTimer(1.5f).Timeout += () =>
		{
			ExecuteWithBlackscreen(0.25f,
				duringBlackscreen: () =>
				{
					LevelManager.Instance.LoadNextLevel();
					PlayerController.Instance.MovePlayerToSpawn();
				},
				afterBlackscreen: () =>
				{
					ResetCurrentLevel();
					EffectManager.Instance.BlackscreenEffect.Disable();
				},
				fakeDelay: 1f
			);
		};

	}

	public void ResetCurrentLevel()
	{
		ResetLevelEvent?.Invoke();
		PlayerController.Instance.MovePlayerToSpawn();
		PlayerController.Instance.ReleasePlayer();
		EffectManager.Instance.GrayscaleEffect.Disable();		
	}

	public void ResetCurrentLevelAfterDeath()
	{
		ExecuteWithBlackscreen(0.25f,
			duringBlackscreen: ResetCurrentLevel,
			afterBlackscreen: () =>
			{
				InputManager.Instance.SetInputContext(new GameInputContext());
			}
		);
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
			EffectManager.Instance.BlackscreenEffect.Disable(fadeTime);

			if (afterBlackscreen != null)
			{
				GetTree().CreateTimer(fadeTime).Timeout += afterBlackscreen;
			}
		}

		EffectManager.Instance.BlackscreenEffect.Enable(fadeTime);
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

	public void SignalStarCollected(CollectibleStar star)
	{
		
	}

    public void OnSave(SavegameData save)
    {
        throw new NotImplementedException();
    }

    public void OnLoad(SavegameData save)
    {
        throw new NotImplementedException();
    }

}
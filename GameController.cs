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

	// Level
	public float LevelStartTime { get; private set; }

	public float LevelTime => GameTime - LevelStartTime;

	public bool LevelRunning { get; private set; } = false;
	public int CurrentLevelIndex { get; private set; } = 0;

	public override void _EnterTree()
	{
		Instance = this;
		this.SubscribeAsSaveable();
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
			EffectManager.Instance.ExecuteWithBlackscreen(0.25f,
				duringBlackscreen: () =>
				{
					CurrentLevelIndex++;
					SavegameManager.Instance.TriggerAutosave();
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
		LevelStartTime = GameTime;
		SavegameManager.Instance.TriggerAutosave();
	}

	public void ResetCurrentLevelAfterDeath()
	{
		EffectManager.Instance.ExecuteWithBlackscreen(0.25f,
			duringBlackscreen: ResetCurrentLevel,
			afterBlackscreen: () =>
			{
				InputManager.Instance.SetInputContext(new GameInputContext());
			}
		);
	}



	public void SignalStarCollected(CollectibleStar star)
	{
		
	}

	public void OnSave(SavegameData save)
	{
		save.CurrentRun.CurrentLevel = CurrentLevelIndex;
	}

	public void OnLoad(SavegameData save)
	{
		CurrentLevelIndex = (save.CurrentRun?.CurrentLevel).GetValueOrDefault(0);
	}

}

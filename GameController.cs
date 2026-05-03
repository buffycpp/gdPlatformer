using Godot;
using System;

public partial class GameController : Node
{
	public static GameController Instance { get; private set; }

	public event Action StartLevelEvent;
	public event Action PlayerDeathEvent;
	public event Action PlayerSpawnEvent;

	public static float GameTime { get; private set; }

	public float TimeScale { get; set; } = 1f;

	// Level timing
	public float LevelStartTime { get; private set; }

	public float LevelTime => GameTime - LevelStartTime;

	public bool LevelRunning { get; private set; } = false;

	public override void _EnterTree()
	{
		Instance = this;
	}

    public override void _Ready()
    {
        StartLevel();
    }


	public override void _Process(double delta)
	{
		float scaledDelta = (float)delta * TimeScale;
		GameTime += scaledDelta;
	}

	public void StartLevel()
	{
		LevelStartTime = GameTime;
		LevelRunning = true;
		StartLevelEvent?.Invoke();
	}

	public void StopLevel()
	{
		LevelRunning = false;
	}

	public void HandleDamage(DamageSource source)
	{
		PlayerDeathEvent?.Invoke();
	}

	public void SpawnPlayer()
	{
		StartLevel();
		PlayerSpawnEvent?.Invoke();
	}
}
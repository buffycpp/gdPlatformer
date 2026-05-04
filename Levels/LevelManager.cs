using Godot;
using System;

public partial class LevelManager : Node
{
	[Export] public LevelList ActiveLevelList { get; set; }
	[Export] public Node ActiveLevelContainer { get; set; }

	public static LevelManager Instance { get; private set; }
	private Node2D activeLevel;
	private int currentLevelIndex = -1;

	public override void _EnterTree()
	{
		Instance = this;
	}

	public override void _Ready()
	{
		if (ActiveLevelList?.Levels == null || ActiveLevelList.Levels.Length == 0)
		{
			GD.PrintErr("No levels assigned in LevelList!");
			return;
		}
	}

	private void ClearCurrentLevel()
	{
		if (activeLevel != null)
		{
			activeLevel.QueueFree();
			activeLevel = null;
		}
	}

	private void SpawnLevel(LevelData levelData)
	{
		if (levelData == null || levelData.LevelScene == null)
		{
			GD.PrintErr("Invalid LevelData or missing scene.");
			return;
		}

		activeLevel = levelData.LevelScene.Instantiate<Node2D>();
		ActiveLevelContainer.AddChild(activeLevel);
	}

	public void LoadLevelByIndex(int index)
	{
		if (ActiveLevelList?.Levels == null)
			return;

		if (index < 0 || index >= ActiveLevelList.Levels.Length)
		{
			GD.PrintErr($"Invalid level index: {index}");
			return;
		}

		ClearCurrentLevel();

		currentLevelIndex = index;
		SpawnLevel(ActiveLevelList.Levels[index]);
	}

	public void LoadLevelById(string id)
	{
		if (ActiveLevelList?.Levels == null)
			return;

		for (int i = 0; i < ActiveLevelList.Levels.Length; i++)
		{
			if (ActiveLevelList.Levels[i]?.Id == id)
			{
				LoadLevelByIndex(i);
				return;
			}
		}

		GD.PrintErr($"Level ID not found: {id}");
	}

	public void LoadNextLevel()
	{
		LoadLevelByIndex(currentLevelIndex + 1);
	}

	public void ReloadCurrentLevel()
	{
		if (currentLevelIndex >= 0)
			LoadLevelByIndex(currentLevelIndex);
	}
}
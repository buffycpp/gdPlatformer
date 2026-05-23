using Godot;
using System;

public partial class MainMenu : Node2D
{
	[Export]public Button ContinueButton { get; private set; }
	[Export] public Button NewGameButton { get; private set; }
	[Export] public Button SettingsButton { get; private set; }
	[Export] public Button ExitButton { get; private set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SavegameManager.Instance.Initialise();

		if (SavegameManager.Instance.CurrentSave.CurrentRun != null)
		{
			ContinueButton.Disabled = false;
			ContinueButton.Text = "Continue (Level " + (SavegameManager.Instance.CurrentSave.CurrentRun.CurrentLevel + 1) + ")";
		}

		ContinueButton.Pressed += OnContinuePressed;
		NewGameButton.Pressed += OnNewGamePressed;
		SettingsButton.Pressed += OnSettingsPressed;
		ExitButton.Pressed += OnExitPressed;
	}

	private void OnExitPressed()
	{
		GetTree().Quit();
	}


	private void OnSettingsPressed()
	{
		throw new NotImplementedException();
	}


	private void OnNewGamePressed()
	{
		EffectManager.Instance.ExecuteWithBlackscreen(0.25f, duringBlackscreen: () =>
		{
			SavegameManager.Instance.CurrentSave.StartNewGame();
			GetTree().ChangeSceneToFile("res://Scenes/main.tscn");
		});

	}


	private void OnContinuePressed()
	{
		EffectManager.Instance.ExecuteWithBlackscreen(0.25f, duringBlackscreen: () =>
		{
			GetTree().ChangeSceneToFile("res://Scenes/main.tscn");
		});		
	}

	public override void _Process(double delta)
	{
	}
}

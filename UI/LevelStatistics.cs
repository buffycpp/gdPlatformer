using Godot;
using System;
using System.Diagnostics;

public partial class LevelStatistics : Node, ISaveable
{
	[Export] public RichTextLabel levelTimeLabel;
	[Export] public Label levelDeathsLabel;

	private int _deathCount = 0;
	public float TotalLevelTime { get; private set; } = 0;

	public override void _EnterTree()
	{
		this.SubscribeAsSaveable();
	}
	
	public override void _Ready()
	{
		GameController.Instance.PlayerDeathEvent += OnPlayerDeath;
		GameController.Instance.ResetLevelEvent += OnResetLevel;
	}

	private void OnResetLevel()
	{
		TotalLevelTime += GameController.Instance.LevelTime;
	}


	private void OnPlayerDeath()
	{
		_deathCount++;
		levelDeathsLabel.Text = _deathCount.ToString();		
	}
	
	public override void _Process(double delta)
	{
		TimeSpan ts = TimeSpan.FromSeconds(GameController.Instance.LevelTime);
		levelTimeLabel.Text = TimeSpan.FromSeconds(TotalLevelTime).ToString(@"mm\:ss\.f") + " [font_size=24][color=#23cc77]+" + ts.ToString(@"mm\:ss\.f") + "[/color][/font_size]";
	}

	public void OnSave(SavegameData save)
	{
		save.CurrentRun.LevelDeaths = _deathCount;
		save.CurrentRun.TotalTime = TotalLevelTime;
	}

	public void OnLoad(SavegameData save)
	{
		_deathCount = (save.CurrentRun?.LevelDeaths).GetValueOrDefault(0);
		TotalLevelTime = (save.CurrentRun?.TotalTime).GetValueOrDefault(0);
		levelDeathsLabel.Text = _deathCount.ToString();
	}

}

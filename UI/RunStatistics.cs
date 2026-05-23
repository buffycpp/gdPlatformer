using Godot;
using System;
using System.Diagnostics;

public partial class RunStatistics : Node
{
	[Export] public Label levelTimeLabel;
	[Export] public Label levelDeathsLabel;

	private int _deathCount = 0;
	
	public override void _Ready()
	{
		GameController.Instance.PlayerDeathEvent += OnPlayerDeath;
	}

    private void OnPlayerDeath()
    {
        _deathCount++;
		levelDeathsLabel.Text = _deathCount.ToString();
    }
    
    public override void _Process(double delta)
	{
		TimeSpan ts = TimeSpan.FromSeconds(GameController.Instance.LevelTime);
		levelTimeLabel.Text = ts.ToString(@"mm\:ss\.f");
	}
}

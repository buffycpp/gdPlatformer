using Godot;
using System;

public partial class TimeSyncedNode2D : Node2D
{
	[Export] public float WakeUpDelay = 0;

	protected bool _isAwake = false;

	public override void _Ready()
	{
		GameController.Instance.StartLevelEvent += OnStartLevel;
	}

    private void OnStartLevel()
    {
		if (WakeUpDelay <= 0)
		{
        	_isAwake = true;			
			OnWakeUp();
		}
    }

    public override void _Process(double delta)
	{
		if (_isAwake)
		{
			return;
		}

		if (GameController.Instance.LevelTime >= WakeUpDelay)
		{
			_isAwake = true;
			OnWakeUp();
		}
	}

	protected virtual void OnWakeUp()
	{
		
	}

    public override void _ExitTree()
    {
		GameController.Instance.StartLevelEvent -= OnStartLevel;
    }

}

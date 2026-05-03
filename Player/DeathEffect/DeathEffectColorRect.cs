using Godot;
using System;

public partial class DeathEffectColorRect : ColorRect
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GameController.Instance.PlayerDeathEvent += OnPlayerDeath;
		GameController.Instance.PlayerSpawnEvent += OnPlayerSpawn;
	}

    private void OnPlayerDeath()
    {
        Visible = true;
    }

    private void OnPlayerSpawn()
    {
        Visible = false;
    }	

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

    public override void _ExitTree()
    {
        GameController.Instance.PlayerDeathEvent -= OnPlayerDeath;
		GameController.Instance.PlayerSpawnEvent -= OnPlayerSpawn;
    }

}

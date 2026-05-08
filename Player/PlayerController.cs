using Godot;
using System;

public partial class PlayerController : SingletonNode<PlayerController>
{
    [Export] public Player Player { get; set; }
    [Export] public PlayerCamera PlayerCamera { get; set; }
    [Export] public float InputBlockTimeAfterDeath = 0.5f;

    public override void _Ready()
    {
        GameController.Instance.PlayerDeathEvent += OnPlayerDeath;
        GameController.Instance.LevelCompleteEvent += OnLevelComplete;
    }

    private void OnLevelComplete()
    {
        Player.SetDormance(true);
        Player.ResetInput();
    }

    private void OnPlayerDeath()
    {
        Player.SetDeath(true);
        Player.ResetInput();
    }

    /// <returns>True if the event was succesfully consumed</returns>
    public void TryJump()
    {
        if (IsInputBlocked())
        {
            return;
        }

        Player.QueueJump();
    }

    public void WalkTo(Vector2 position, Action onDestinationReached = null)
    {
        Player.WalkTo(position, onDestinationReached);
    }

    public void MoveTo(Vector2 position, bool withCamera = false)
    {
        Player.ForcePosition(position);

        if (withCamera)
        {
            PlayerCamera.ForcePosition(position);
        }
    }

    public void MovePlayerToSpawn()
    {
        PlayerController.Instance.PlayerCamera.IsPaused = false;
        var position = new Vector2(0, 0);
        MoveTo(position, withCamera: true);
    }

    public void ReleasePlayer()
    {
        Player.SetDeath(false);
        Player.SetDormance(false);
    }

    public void SetXInput(float input)
    {
        float x = input;
        if (IsInputBlocked())
        {
            x = 0;
        }

        if (x != 0)
        {
            SignalLevelStart();
        }

        Player.SetXInput(x);
    }

    public bool IsInputDeathBlocked()
    {
        return Player.AliveTime < InputBlockTimeAfterDeath;
    }

    public bool IsInputBlocked()
    {
        return Player.IsDead || Player.IsDormant || IsInputDeathBlocked();
    }

    public void SignalLevelStart()
    {
        GameController.Instance.StartLevel();
    }

    public void TryInteract()
    {
        Player?.PlayerInteraction?.TriggerInteraction();
    }

    public override void _ExitTree()
    {
        GameController.Instance.PlayerDeathEvent -= OnPlayerDeath;
    }
}

using Godot;
using System;

public partial class Switch : Area2D, IInteractable
{
	[Export] public Sprite2D OnSprite;
	[Export] public Sprite2D OffSprite;
	[Export] public AudioStream ToggleSound;
	[Export] public Node TriggerableNode;

	public ITriggerable Triggerable => TriggerableNode as ITriggerable;

	public bool isOn = false;

	public override void _Ready()
	{
		GameController.Instance.ResetLevelEvent += OnResetLevel;
	}

    private void OnResetLevel()
    {
        Toggle(false, reset: true);
    }

    public override void _Process(double delta)
	{
	}

	public void Toggle(bool on, bool reset = false)
	{
		isOn = on;
		OnSprite.Visible = on;
		OffSprite.Visible = !on;

		if (!reset)
		{
			SoundManager.Instance.PlaySfx(ToggleSound, GlobalPosition);			
		}
	}

    public bool CanInteract()
    {
        return !isOn;
    }

    public void Interact()
    {
        Toggle(true);
		if ((Triggerable?.CanTrigger()).GetValueOrDefault())
		{
			Triggerable.Trigger();			
		}
    }

	public override void _ExitTree()
	{
		GameController.Instance.ResetLevelEvent -= OnResetLevel;
	}

}

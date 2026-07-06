using Godot;
using System;

public partial class Door : StaticBody2D, ITriggerable
{
	[Export] public AnimatedSprite2D animatedSprite2D;
	[Export] public CollisionShape2D collisionShape2D;
	[Export] public AudioStream ToggleSound;	
	
	public override void _Ready()
	{
		GameController.Instance.ResetLevelEvent += OnResetLevel;
	}

	private void OnResetLevel()
	{
		Close();
	}


	public void Open()
	{
		animatedSprite2D.Play("open");
		collisionShape2D.Disabled = true;
		SoundManager.Instance.PlaySfx(ToggleSound, GlobalPosition);
	}

	public void Close()
	{
		animatedSprite2D.Play("closed");
		collisionShape2D.Disabled = false;
	}

	public bool CanTrigger()
	{
		return animatedSprite2D.Animation == "closed";
	}

	public void Trigger(string actionName)
	{
		Open();
	}

	public override void _ExitTree()
	{
		GameController.Instance.ResetLevelEvent -= OnResetLevel;
	}
}

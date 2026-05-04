using Godot;
using System;

public partial class InputManager : SingletonNode<InputManager>
{
	private IInputContext _activeContext;

	public override void _Ready()
	{
		SetInputContext(new GameInputContext());
	}

	public override void _Process(double delta)
	{
		_activeContext?.ProcessInput(delta);
	}

	public void SetInputContext(IInputContext inputContext)
	{
		_activeContext = inputContext;
	}

	public void SetInputContextDelayed(IInputContext inputContext, float delay, bool nullDuringTransition = true)
	{
		if (nullDuringTransition)
		{
			SetInputContext(null);
		}

		GetTree().CreateTimer(0.5f).Timeout += () =>
		{
			SetInputContext(inputContext);
		};
	}
}
